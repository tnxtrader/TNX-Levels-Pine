// COMPILE-ONLY STUB of TradingPlatform.BusinessLayer (Quantower API) - NOT the real API. See AssemblyInfo.cs for the legend.
using System;
using System.Collections.Generic;
using System.Drawing;

namespace TradingPlatform.BusinessLayer
{
    /// <summary>
    /// VERIFIED(IL): .ctor(string, int, double, double, double, int, object[]) - the user's source always passes all 7.
    /// Parameter NAMES and defaults VERIFIED by official usage: [InputParameter], [InputParameter("x", 10)],
    /// [InputParameter("Period", 0, 1, 1000)], [InputParameter("Value area", minimum: 0, maximum: 100)],
    /// [InputParameter("Fast MA", 2, minimum: 1, maximum: 100, increment: 1, decimalPlaces: 0)], [InputParameter("x", 1, variants: new object[]{...})].
    /// The decompiled defaults in the user's source for bool/Color fields are (int.MinValue, int.MaxValue, 0.01, 2).
    /// VERIFIED(IL): get_Name() : string, get_Variants() : IComparable[].
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class InputParameterAttribute : Attribute
    {
        public InputParameterAttribute(string name = null, int sortIndex = 0, double minimum = int.MinValue, double maximum = int.MaxValue, double increment = 0.01, int decimalPlaces = 2, object[] variants = null)
        {
            this.Name = name;
            this.SortIndex = sortIndex;
            this.Minimum = minimum;
            this.Maximum = maximum;
            this.Increment = increment;
            this.DecimalPlaces = decimalPlaces;
            if (variants != null)
            {
                var arr = new IComparable[variants.Length];
                for (int i = 0; i < variants.Length; i++)
                    arr[i] = variants[i] as IComparable;
                this.Variants = arr;
            }
        }

        public string Name { get; }             // VERIFIED(IL)
        public int SortIndex { get; }           // ASSUMED name
        public double Minimum { get; }          // ASSUMED name
        public double Maximum { get; }          // ASSUMED name
        public double Increment { get; }        // ASSUMED name
        public int DecimalPlaces { get; }       // ASSUMED name
        public IComparable[] Variants { get; }  // VERIFIED(IL) IComparable[]
    }

    public enum VisibilityMode
    {
        Visible,   // VERIFIED(ex:UsingInnerIndicatorSettings.cs)
        Hidden,    // ASSUMED
    }

    /// <summary>VERIFIED(IL): SelectItem.get_Value() : IComparable. Ctor (string, value) VERIFIED(ex:SettingsRelationsExample.cs).</summary>
    public class SelectItem
    {
        public SelectItem(string text, IComparable value) { this.Text = text; this.Value = value; }   // VERIFIED(ex) new SelectItem("First option", 1)
        public string Text { get; set; }        // ASSUMED name
        public IComparable Value { get; set; }  // VERIFIED(IL)
    }

    /// <summary>Base for Relation = new SettingItemRelationVisibility(...). VERIFIED(ex:SettingsRelationsExample.cs).</summary>
    public abstract class SettingItemRelation
    {
        protected SettingItemRelation(string settingItemName, params SelectItem[] items) { this.SettingItemName = settingItemName; this.Items = items; }
        public string SettingItemName { get; }   // ASSUMED name
        public SelectItem[] Items { get; }       // ASSUMED name
    }
    public class SettingItemRelationVisibility : SettingItemRelation   // VERIFIED(ex:SettingsRelationsExample.cs)
    {
        public SettingItemRelationVisibility(string settingItemName, params SelectItem[] items) : base(settingItemName, items) { }
    }
    public class SettingItemRelationEnability : SettingItemRelation    // VERIFIED(ex:SettingsRelationsExample.cs)
    {
        public SettingItemRelationEnability(string settingItemName, params SelectItem[] items) : base(settingItemName, items) { }
    }

    /// <summary>VERIFIED(ex:UsingInnerIndicatorSettings.cs): new SettingItemSeparatorGroup("Super Trend"); .Text.</summary>
    public class SettingItemSeparatorGroup
    {
        public SettingItemSeparatorGroup(string text) { this.Text = text; }
        public string Text { get; set; }
    }

    /// <summary>VERIFIED(IL): get_Name() : string, get_Value() : object. Text/SortIndex/Relation/SeparatorGroup/VisibilityMode VERIFIED(ex).</summary>
    public class SettingItem
    {
        public SettingItem() { }
        public SettingItem(string name, object value) { this.Name = name; this.Value = value; }
        public string Name { get; set; }                                   // VERIFIED(IL)
        public virtual object Value { get; set; }                          // VERIFIED(IL) object
        public string Text { get; set; }                                   // VERIFIED(ex:SettingsRelationsExample.cs)
        public int SortIndex { get; set; }                                 // VERIFIED(ex:SettingsRelationsExample.cs)
        public SettingItemRelation Relation { get; set; }                  // VERIFIED(ex:SettingsRelationsExample.cs)
        public SettingItemSeparatorGroup SeparatorGroup { get; set; }      // VERIFIED(ex:UsingInnerIndicatorSettings.cs)
        public VisibilityMode VisibilityMode { get; set; }                 // VERIFIED(ex:UsingInnerIndicatorSettings.cs)
    }

    public class SettingItemInteger : SettingItem      // VERIFIED(ex:AccessIndicatorByName.cs) new SettingItemInteger("MaPeriod", 10)
    {
        public SettingItemInteger(string name, int value) : base(name, value) { }
    }
    public class SettingItemDouble : SettingItem       // VERIFIED(ex:SettingsRelationsExample.cs)
    {
        public SettingItemDouble(string name, double value) : base(name, value) { }
    }
    public class SettingItemString : SettingItem       // VERIFIED(ex:SettingsRelationsExample.cs)
    {
        public SettingItemString(string name, string value) : base(name, value) { }
    }
    public class SettingItemDateTime : SettingItem     // VERIFIED(ex:SettingsRelationsExample.cs)
    {
        public SettingItemDateTime(string name, DateTime value) : base(name, value) { }
    }
    public class SettingItemBoolean : SettingItem      // ASSUMED (by analogy)
    {
        public SettingItemBoolean(string name, bool value) : base(name, value) { }
    }
    public class SettingItemColor : SettingItem        // ASSUMED (by analogy)
    {
        public SettingItemColor(string name, Color value) : base(name, value) { }
    }
    /// <summary>VERIFIED(ex:SettingsRelationsExample.cs): new SettingItemSelectorLocalized(name, value, new List&lt;SelectItem&gt;{...}). Scripts pass an enum as value, so it is object.</summary>
    public class SettingItemSelectorLocalized : SettingItem
    {
        public SettingItemSelectorLocalized(string name, object value, IList<SelectItem> items) : base(name, value) { this.Items = items; }
        public IList<SelectItem> Items { get; set; }   // ASSUMED name
    }
    /// <summary>VERIFIED(ex:UsingInnerIndicatorSettings.cs): new SettingItemGroup("SuperTrend", list); (List&lt;SettingItem&gt;)group.Value.</summary>
    public class SettingItemGroup : SettingItem
    {
        public SettingItemGroup(string name, IList<SettingItem> items) : base(name, items) { }
    }

    /// <summary>Extension helpers on IList&lt;SettingItem&gt;. Static class name ASSUMED; methods VERIFIED(ex).</summary>
    public static class SettingItemExtensions
    {
        public static bool TryGetValue<T>(this IList<SettingItem> settings, string name, out T value)   // VERIFIED(ex:SettingsRelationsExample.cs)
        {
            var item = GetItemByName(settings, name);
            if (item != null && item.Value is T t) { value = t; return true; }
            value = default(T);
            return false;
        }

        public static SettingItem GetItemByName(this IList<SettingItem> settings, string name)          // VERIFIED(ex:UsingInnerIndicatorSettings.cs)
        {
            if (settings == null) return null;
            foreach (var s in settings)
                if (s != null && s.Name == name) return s;
            return null;
        }
    }
}
