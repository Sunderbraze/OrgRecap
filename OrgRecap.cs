using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using FullSerializer;
using HarmonyLib;
using PavonisInteractive.TerraInvicta;
using PavonisInteractive.TerraInvicta.Entities;
using PavonisInteractive.TerraInvicta.Systems.GameTime;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using UnityModManagerNet;

[assembly: AssemblyVersion ("1.0.0.0")]
[assembly: AssemblyTitle ("OrgRecap")]
[assembly: AssemblyDescription ("Allows changing of org caps in Terra Invicta")]
[assembly: AssemblyConfiguration ("")]
[assembly: AssemblyCompany ("")]
[assembly: AssemblyProduct ("")]
[assembly: AssemblyCopyright ("")]
[assembly: AssemblyTrademark ("")]
[assembly: ComVisible (false)]
[assembly: Guid ("eb2dda6f-62b0-483e-89e6-78dc1a7b5e11")]
[assembly: AssemblyFileVersion ("1.0.0.0")]

namespace OrgRecap
{

	internal static class Main
	{

		private static bool Load(UnityModManager.ModEntry modEntry)
		{
			Harmony harmony = new Harmony(modEntry.Info.Id);
			harmony.PatchAll(Assembly.GetExecutingAssembly());
			Main.mod = modEntry;
			modEntry.OnGUI = OnGUI;
			modEntry.OnToggle = new Func<UnityModManager.ModEntry, bool, bool>(Main.OnToggle);
			return true;
		}

		private static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
		{
			Main.enabled = value;
			return true;
		}

		public static bool enabled;
		public static string orgGarageCapStr = "10";
		public static string orgCouncilorCapStr = "15";
		public static int orgGarageCapInt = 10;
		public static int orgCouncilorCapInt = 15;
		public static UnityModManager.ModEntry mod;

    private static void OnGUI(UnityModManager.ModEntry modEntry)
		{
			GUILayout.Label("Councilor Org Cap");
			Main.orgCouncilorCapStr = GUILayout.TextField(Main.orgCouncilorCapStr, GUILayout.Width(100f));
			GUILayout.Label("Unassigned Org Cap");
			Main.orgGarageCapStr = GUILayout.TextField(Main.orgGarageCapStr, GUILayout.Width(100f));
			int cCap = 15;
			int gCap = 10;
			if (GUILayout.Button("Apply") && int.TryParse(Main.orgCouncilorCapStr, out cCap) && int.TryParse(Main.orgGarageCapStr, out gCap))
			{
				Main.orgCouncilorCapInt = cCap;
				Main.orgGarageCapInt = gCap;
			}
		}

		[HarmonyPatch(typeof(TIFactionState), "UnassignedPoolOverage")]
		private static class Patch
		{

			private static void Postfix(TIFactionState __instance, ref int __result)
			{
				__result = Mathf.Max(__instance.unassignedOrgs.Count - (from x in __instance.unassignedOrgs
				where !x.template.allowedOnMarket
				select x).Count<TIOrgState>() - Main.orgGarageCapInt, 0);
			}

		}

		[HarmonyPatch(typeof(TICouncilorState), "SufficientCapacityForOrg")]
		private static class Patch2
		{

			private static void Postfix(TIOrgState org, TICouncilorState __instance, ref bool __result)
			{
				__result = __instance.orgs.Count < Main.orgCouncilorCapInt && __instance.availableAdministration >= org.tier - org.administration && __instance.orgsWeight + org.tier <= TemplateManager.global.maxCouncilorAttribute;
			}

		}

  }

}