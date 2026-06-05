# Warzone Cat Ears v2.0.1

Author: RaiRaiTheRaichu

Additional credits:
- SamSWAT for the initial mod this is inherited from, "Crye Precision Airframe Helmet From Warzone".

### ---BUILT FOR SPT VERSION 4.0.13---

We all know SPT needs more accessories. What's the point of all the killing if you aren't going to look good while you do it?

I've taken the same mesh for the Cat Ear headband from SamSWAT's "Crye Precision Airframe Helmet From Warzone" mod and edited the mesh to fit and look proper in the SLAAP plate slot of any helmet that can accept one. I've also added a few alternate textures, in case blue isn't your thing.
We've got blue, pink, red, green, and yellow.

Oh, and of course you don't have to forego protection just for style. Enabled by default, I have added a config option to copy the protection/hitbox of a normal SLAAP plate, so you don't have to lose out on your armor. Does it make sense? Probably not, but who cares? Cat ears, bro.

Ня.

## ---BUILD INFO---

Requirements:
- .NET 9.0

The following packages installed:
- SPTarkov.Common (`4.0.0`)
- SPTarkov.DI (`4.0.0`)
- SPTarkov.Reflection (`4.0.0`)
- SPTarkov.Server.Core (`4.0.0`)
(You can select `Manage NuGet Packages...` within the Dependencies tab of Visual Studio to install them. Pay special attention to the version being installed, mods compiled with packages marked `4.0.13`, for example, will not run on any version below that.)

Open WZCatEars.sln with Visual Studio and build the mod in Release mode.
Alternatively, run `dotnet build -c Release` from the project directory root.

The output will be stored in the project's `bin\Release\` folder.

BUNDLES ARE NOT INCLUDED ON THE REPO DUE TO FILE SIZE CONCERNS, please download them and add them to the WZCatEars mod folder manually.
https://drive.google.com/file/d/1VuJfiC7yiatkj6igNX-kS-oiFSS9wddx/view

I will try to keep this link accurate and up-to-date with the required bundles to run the mod.

## ---INSTALL INFO---

How to install:
Drag and drop the included `SPT` folder from this zip into your SPT folder, allow it to merge with your existing folder.

If you're updating from an older version, please be sure to delete the old mod from your folder.


## ---CHANGELOGS---

#### v1.1.0 Changelog: 
- Config for setting the ears to cosmetic-only will also lower the price.
- Refactor to module changes for SPT 3.9.0+.
- Minor change to the version in the package.json, compatible with version 3.9.0 of SPT and above.

#### v1.1.1 Changelog: 
- Minor change to the version in the package.json, compatible with version 3.10.0 of SPT and above.

#### v2.0.0 Changelog:
- Rewrite for SPT version 4.0+.
- Added advanced trade offer customization in the config.

#### v2.0.1 Changelog:
- Fixed an OS-dependent issue with loading database and config files

## ---CONTACT---

@RaiRaiTheRaichu - Discord
https://forge.sp-tarkov.com/user/3576/rairaitheraichu

## ---LICENSE---

Copyright 2026 - RaiRaiTheRaichu

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.