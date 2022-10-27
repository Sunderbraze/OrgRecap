
# Usage

This mod has a settings menu in Unity Mod Manager (required, see below) that enables you to change the number of orgs that can be assigned to a councilor and the number of orgs allowed in a faction's unassigned org pool. Unity Mod Manager opens when the game launches, then you can adjust the values by clicking the settings button next to the mod.

Note that the UI will always say 15 is the maximum number of orgs, regardless of what the actual value is. This is caused by a technical inefficiency in the code that I explain further in the "Why" section below.

# Installation

Unfortunately this mod requires Unity Mod Manager to function because the game currently will not load assemblies from mod folders. Hopefully in the future it will do so, like Rimworld and many others.

Unity Mod Manager homepage: https://www.nexusmods.com/site/mods/21
 
Alternate D/L link because nexus mods is trash: https://www.dropbox.com/s/wz8x8e4onjdfdbm/UnityModManager.zip?dl=1

Remember to install Unity Mod Manager using Doorstop Proxy installation method.

# Why a C# assembly? Why not JSON?

I'm glad you asked. Currently, there are two aspects of Orgs that are baked into the game's assembly code and therefore cannot be modified with JSON files: the number of orgs that can be assigned to a councilor, and the number of orgs allowed in a faction's unassigned org pool. 

If any devs happen to read this, these specific functions are the issue:


PavonisInteractive.TerraInvicta.TICouncilorState.SufficientCapacityForOrg(TIOrgState) : bool @060024C4

  return this.orgs.Count < 15 && this.availableAdministration >= org.tier - org.administration && this.orgsWeight + org.tier <= this.maxCouncilorAttribute;

Instead of 15, this should reference an int defined in TIGlobalConfig, perhaps councilorMaxOrgCapacity


PavonisInteractive.TerraInvicta.TIFactionState.UnassignedPoolOverage() : int @060029B6

  return Mathf.Max(this.unassignedOrgs.Count - (from x in this.unassignedOrgs where !x.template.allowedOnMarket select x).Count<TIOrgState>() - 10, 0);

Instead of 10, this should reference an int defined in TIGlobalConfig, perhaps factionMaxUnassignedOrgs

# Sauce?

Of course.

Unity Mod Manager: https://github.com/newman55/unity-mod-manager/

Org Recap: https://github.com/Sunderbraze/OrgRecap/

# Help! Something doesnt work!

I don't know if I'll have the time of day to help everyone who has issues, but feel free to post in the Steam Workshop comments. I'll respond if I can. Tech support is my day job so you'll forgive me for being unenthusiastic about doing it for free in my downtime :)
