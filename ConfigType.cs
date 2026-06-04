using SPTarkov.Server.Core.Models.Common;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;

namespace WZCatEars
{
    public class ConfigType
    {
        public bool ArmoredCatEars { get; set; }
        public bool UseAlternateTrade { get; set; }
        public AlternateTradeConfigType? AlternateTrade { get; set; }
    }

    public class AlternateTradeConfigType
    {
        public MongoId TraderId { get; set; }
        public TradeRequirementConfigType Barter { get; set; }
        public int LoyaltyLevel { get; set; }
    }

    public class TradeRequirementConfigType
    {
        public MongoId Id { get; set; }
        public int Count { get; set; }
    }

    public class ItemEntryType
    {
        public MongoId Id { get; set; }
        public MongoId Template { get; set; }
        public string BundlePath { get; set; }
    }
}
