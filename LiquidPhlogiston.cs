using System;
using System.Collections.Generic;
using System.Text;
using Qud.API;
using XRL.Core;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;

using XRL.Liquids;

[Serializable]
[IsLiquid]
public class LiquidPhlogiston : BaseLiquid
{
	public new const string ID = "phlogiston";

	[NonSerialized]
	public static List<string> Colors = new List<string>(3) { "O", "W", "y" };

	public LiquidLava() : base("phlogiston")
	{
		Weight = 0.1;
		InterruptAutowalk = false;
		ConsiderDangerousToContact = false;
		ConsiderDangerousToDrink = true;
		Glows = false;
	}

	public override List<string> GetColors()
	{
		return Colors;
	}

	public override string GetColor()
	{
		return "O";
	}

	public override string GetName(LiquidVolume Liquid)
	{
		return "phlogiston";
	}

	public override string GetAdjective(LiquidVolume Liquid)
	{
		return "phlogistonated";
	}

	public override string GetWaterRitualName()
	{
		return "phlogiston";
	}

	public override string GetSmearedAdjective(LiquidVolume Liquid)
	{
		return "phlogistonated";
	}

	public override string GetSmearedName(LiquidVolume Liquid)
	{
		return "phlogiston-covered";
	}

	public override string GetStainedName(LiquidVolume Liquid)
	{
		return "phlogiston";
	}

	public override float GetValuePerDram()
	{
		return 1.0f;
	}

	public override bool Drank(LiquidVolume Liquid, int Volume, GameObject Target, StringBuilder Message, ref bool ExitInterface)
	{
		Message.Compound("It tastes fiery.");
		ExitInterface = true;
		return true;
	}

	public override void BaseRenderPrimary(LiquidVolume Liquid)
	{
		Liquid.ParentObject.pRender.ColorString = "&W^R";
		Liquid.ParentObject.pRender.TileColor = "&W";
		Liquid.ParentObject.pRender.DetailColor = "R";
	}

	public override void BaseRenderSecondary(LiquidVolume Liquid)
	{
		Liquid.ParentObject.pRender.ColorString += "&R";
	}

	public override void RenderPrimary(LiquidVolume Liquid, RenderEvent eRender)
	{
		if (!Liquid.IsWadingDepth())
		{
			return;
		}
		if (Liquid.ParentObject.IsFrozen())
		{
			eRender.RenderString = "~";
			eRender.TileVariantColors("&W^R", "&W", "R");
			return;
		}
		Render pRender = Liquid.ParentObject.pRender;
		int num = (XRLCore.CurrentFrame + Liquid.nFrameOffset) % 60;
		if (Stat.RandomCosmetic(1, 600) == 1)
		{
			eRender.RenderString = "\u000f";
			eRender.TileVariantColors("&W^R", "&W", "R");
		}
		if (Stat.RandomCosmetic(1, 60) == 1)
		{
			if (num < 15)
			{
				pRender.RenderString = "÷";
				pRender.ColorString = "&W^R";
				pRender.TileColor = "&W";
				pRender.DetailColor = "R";
			}
			else if (num < 30)
			{
				pRender.RenderString = "~";
				pRender.ColorString = "&W^r";
				pRender.TileColor = "&W";
				pRender.DetailColor = "r";
			}
			else if (num < 45)
			{
				pRender.RenderString = "\t";
				pRender.ColorString = "&W^R";
				pRender.TileColor = "&W";
				pRender.DetailColor = "R";
			}
			else
			{
				pRender.RenderString = "~";
				pRender.ColorString = "&W^R";
				pRender.TileColor = "&W";
				pRender.DetailColor = "R";
			}
		}
	}

	public override void RenderSecondary(LiquidVolume Liquid, RenderEvent eRender)
	{
		if (eRender.ColorsVisible)
		{
			eRender.ColorString += "&r";
		}
	}

	public override string GetPaintAtlas(LiquidVolume Liquid)
	{
		if (Liquid.IsWadingDepth())
		{
			return "Liquids/Splotchy/";
		}
		return base.GetPaintAtlas(Liquid);
	}

	public override int GetNavigationWeight(LiquidVolume Liquid, GameObject GO, bool Smart, bool Slimewalking, ref bool Uncacheable)
	{
		if (Smart && GO != null)
		{
			Uncacheable = true;
			int num = GO.Stat("HeatResistance");
			if (num > 0 && ((!Liquid.IsSwimmingDepth()) ? (!HasFlammableEquipmentEvent.Check(GO, Temperature)) : (!HasFlammableEquipmentOrInventoryEvent.Check(GO, Temperature))))
			{
				if (num >= 100)
				{
					return 0;
				}
				return Math.Min(Math.Max(40 + 59 * (100 - num) / 100, 0), 99);
			}
		}
		return 99;
	}

	public override void StainElements(LiquidVolume Liquid, GetItemElementsEvent E)
	{
		E.Add("might", 1);
	}
}
