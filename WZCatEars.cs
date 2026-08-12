using Microsoft.Extensions.Configuration;
using SPTarkov.Common.Extensions;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Helpers;
using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Spt.Tables;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Servers;
using SPTarkov.Server.Core.Services;
using SPTarkov.Server.Core.Helpers.Server;
using System.Formats.Tar;
using System.Reflection;
using SPTarkov.Common.Models.Logging;

namespace WZCatEars
{
    public record ModMetadata : IModMetadata
    {
        public string ModGuid { get; init; } = "com.rairaitheraichu.wzcatears";
        public string Name { get; init; } = "Warzone Cat Ears";
        public string Author { get; init; } = "RaiRaiTheRaichu";
        public List<string>? Contributors { get; init; }
        public SemanticVersioning.Version Version { get; init; } = new("2.1.0");
        public SemanticVersioning.Range SptVersion { get; init; } = new("~4.1.0");
        public List<string>? Incompatibilities { get; init; }
        public Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; }
        public string? Url { get; init; } = "https://github.com/RaiRaiTheRaichu/SPT_WZ-Cat-Ears-Mod";
        public bool? IsBundleMod { get; init; } = true;
        public string? License { get; init; } = "Apache License V2.0";
        public bool HasPrepatcher { get; init; } = false;
    }

    [Injectable(TypePriority = OnLoadOrder.Preload)]
    public class WZCatEars(
        TemplateTable templateTable,
        TradersTable tradersTable,
        LocaleTable localeTable,
        ModHelper modHelper,
        ISptLogger<WZCatEars> logger) : IOnLoad
    {
        public ConfigType ModConfig = new ConfigType();
        private Dictionary<string, ItemEntryType> ItemEntries = new Dictionary<string, ItemEntryType>();
        public string ModPath;

        public Task OnLoadAsync(CancellationToken cancellationToken)
        {
            // Load config
            ModPath = modHelper.GetAbsolutePathToModFolder(Assembly.GetExecutingAssembly());

            ModConfig = modHelper.GetJsonDataFromFile<ConfigType>(
                System.IO.Path.Join(ModPath, "config"), "config.jsonc");
            ItemEntries = modHelper.GetJsonDataFromFile<Dictionary<string, ItemEntryType>>(
                System.IO.Path.Combine(ModPath, "db"), "item.json");

            GenerateItems();

            cancellationToken.ThrowIfCancellationRequested();

            string[] files = Directory.GetFiles(System.IO.Path.Join(ModPath, "db", "locale"));
            
            if (files.Length > 0) 
                GenerateLocalization(files);

            cancellationToken.ThrowIfCancellationRequested();

            GenerateAssorts();

            cancellationToken.ThrowIfCancellationRequested();

            return Task.CompletedTask;
        }

        private Task GenerateItems()
        {
            var itemDatabase = templateTable.Items;

            foreach (var itemEntry in ItemEntries)
            {
                TemplateItem cloneTemplate = FastCloner.FastCloner.DeepClone(itemDatabase[itemEntry.Value.Template]);
                
                cloneTemplate.Id = itemEntry.Value.Id;
                cloneTemplate.Name = itemEntry.Key;
                cloneTemplate.Properties.Prefab.Path = itemEntry.Value.BundlePath;

                if (!ModConfig.ArmoredCatEars)
                {
                    cloneTemplate.Properties.ArmorClass = null;
                    cloneTemplate.Properties.MaxDurability = null;
                    cloneTemplate.Properties.Indestructibility = null;
                    cloneTemplate.Properties.MaterialType = null;
                    cloneTemplate.Properties.RicochetParams = null;
                    cloneTemplate.Properties.BluntThroughput = null;
                    cloneTemplate.Properties.ArmorMaterial = null;
                    cloneTemplate.Properties.ArmorType = null;
                    cloneTemplate.Properties.ArmorColliders = null;
                    cloneTemplate.Properties.ArmorPlateColliders = null;
                    cloneTemplate.Properties.SpeedPenaltyPercent = null;
                    cloneTemplate.Properties.MousePenalty = null;
                    cloneTemplate.Properties.WeaponErgonomicPenalty = null;
                }

                itemDatabase[cloneTemplate.Id] = cloneTemplate;

                // Add to slots
                var slotList = itemDatabase.Values
                    .Where(item => item.Properties?.Slots != null)
                    .SelectMany(item => item.Properties.Slots)
                    .Where(slot => slot?.Properties?.Filters != null)
                    .SelectMany(slot => slot.Properties.Filters)
                    .Where(filter => filter.Filter.Contains(itemEntry.Value.Template))
                    .ToList();
                foreach (var filter in slotList)
                    filter.Filter.Add(itemEntry.Value.Id);

                // Add to incompatibility arrays
                var incompatibilityList = itemDatabase.Values
                    .Where(item => item.Properties?.ConflictingItems?.Contains(itemEntry.Value.Template) ?? false)
                    .ToList();
                foreach (var conflict in incompatibilityList)
                    conflict.Properties.ConflictingItems.Add(itemEntry.Value.Id);

                // Add to container blacklists
                var excludedList = itemDatabase.Values
                    .Where(item => item.Properties?.Grids != null)
                    .SelectMany(item => item.Properties.Grids)
                    .Where(grid => grid.Properties?.Filters != null)
                    .SelectMany(grid => grid.Properties.Filters)
                    .Where(filter => filter.Filter.Contains(itemEntry.Value.Template))
                    .ToList();
                foreach (var exclusion in excludedList)
                    exclusion.Filter.Add(itemEntry.Value.Id);
            }

            return Task.CompletedTask;
        }

        private Task GenerateLocalization(string[] files)
        { 
            List<string> filenames = new List<string>();

            foreach (string file in files)
            {
                string localeJson = System.IO.Path.GetFileName(file);
                string localePath = System.IO.Path.GetDirectoryName(file);
                string localeKey = System.IO.Path.GetFileNameWithoutExtension(file).ToLower();

                filenames.Add(localeKey);

                var localeFile = modHelper.GetJsonDataFromFile<Dictionary<string, string>>(localePath, localeJson);

                if (localeTable.Global.TryGetValue(localeKey, out var lazyloadedValue))
                {
                    lazyloadedValue.AddTransformer(lazyloadedLocaleData =>
                    {
                        foreach (var localeEntry in localeFile) 
                            lazyloadedLocaleData[localeEntry.Key] = localeEntry.Value;
                        return lazyloadedLocaleData;
                    });
                }
            }

            // Ensure all language options at least have a locale entry, using `en` as a fallback
            foreach (var langKey in localeTable.Global)
            {
                if (!filenames.Contains(langKey.Key))
                {
                    var localeFile = modHelper.GetJsonDataFromFile<Dictionary<string, string>>(
                        System.IO.Path.Join(ModPath, "db", "locale"), "en.json");

                    if (localeTable.Global.TryGetValue(langKey.Key, out var lazyloadedValue))
                    {
                        lazyloadedValue.AddTransformer(lazyloadedLocaleData =>
                        {
                            foreach (var localeEntry in localeFile) 
                                lazyloadedLocaleData[localeEntry.Key] = localeEntry.Value;
                            return lazyloadedLocaleData;
                        });
                    }
                }
            }
            return Task.CompletedTask;
        }

        private Task GenerateAssorts()
        {
            if (ModConfig.UseAlternateTrade)
            {
                foreach (var itemEntry in ItemEntries)
                {
                    var traderAssort = tradersTable[ModConfig.AlternateTrade.TraderId].Assort;

                    Item newItemAssort = new()
                    {
                        Id = new MongoId(),
                        Template = itemEntry.Value.Id,
                        ParentId = "hideout",
                        SlotId = "hideout",
                        Upd = new Upd()
                        {
                            UnlimitedCount = true,
                            StackObjectsCount = 9999999,
                            BuyRestrictionMax = 5,
                            BuyRestrictionCurrent = 0
                        }
                    };

                    BarterScheme newBarterScheme = new()
                    {
                        Count = ModConfig.AlternateTrade.Barter.Count,
                        Template = ModConfig.AlternateTrade.Barter.Id
                    };

                    traderAssort.Items.Add(newItemAssort);
                    traderAssort.BarterScheme[newItemAssort.Id] = [[newBarterScheme]];
                    traderAssort.LoyalLevelItems[newItemAssort.Id] = ModConfig.AlternateTrade.LoyaltyLevel;
                }

                return Task.CompletedTask;
            }

            foreach (var itemEntry in ItemEntries)
            {
                foreach (var trader in tradersTable)
                {
                    if (trader.Value.Assort?.Items == null || trader.Value.Assort?.Items?.Count == 0)
                        continue;
                    
                    var assortList = (trader.Value.Assort?.Items ?? [])
                        .Where(assort => 
                            assort?.Template == itemEntry.Value.Template && 
                            assort?.ParentId == "hideout")
                        .ToList();

                    if (!assortList.Any())
                        continue;
                    
                    foreach (var tradeItem in assortList)
                    {
                        Item newAssort = FastCloner.FastCloner.DeepClone(tradeItem);

                        newAssort.Id = new MongoId();
                        newAssort.Template = itemEntry.Value.Id;

                        trader.Value.Assort.Items.Add(newAssort);
                        trader.Value.Assort.BarterScheme[newAssort.Id] = trader.Value.Assort.BarterScheme[tradeItem.Id];
                        trader.Value.Assort.LoyalLevelItems[newAssort.Id] = trader.Value.Assort.LoyalLevelItems[tradeItem.Id];

                        if (trader.Value.QuestAssort.TryGetValue("success", out var questSuccess)
                            && questSuccess.ContainsKey(tradeItem.Id))
                        {
                            questSuccess.TryAdd(
                                newAssort.Id,
                                questSuccess[tradeItem.Id]
                            );
                        }
                    }
                }

                // Handbook price
                HandbookItem handbookEntry = templateTable.Handbook.Items.Find(entry
                    => entry.Id == itemEntry.Value.Template);

                HandbookItem newHandbookEntry = new()
                {
                    Id = itemEntry.Value.Id,
                    ParentId = handbookEntry.ParentId,
                    Price = handbookEntry.Price
                };

                templateTable.Handbook.Items.Add(newHandbookEntry);
            }
            return Task.CompletedTask;
        }
    }
}
