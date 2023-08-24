using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace AimZoomPatcher
{
    public class ModSettings
    {
        [SettingName("Affects See-Through Scopes")]
        [Tooltip("If enabled, see-through scopes will also be affected by the value below (they don't count as regular scopes for the game).")]
        public bool AffectsScopes { get; set; } = false;

        [Tooltip("Changes the zoom multipier of reflex sights (and other similar sights that are not scopes). If set to 0 no changes will be made. If above 0 it will set the zoom level to the given value, if below 0 it will lower the existing zoom level by the percentage given.\nFor example, if set to -0.75 it will reduce the zoom level by 75%.")]
        public float ReflexSightZoomMultiplier { get; set; } = -0.5f;

        [Tooltip("Changes the zoom multipier of iron sights. If below 1, no changes will be made. A value of 1 will disable iron sight zoom while any value above 1 will set the zoom multiplier to that value.")]
        public float IronSightsZoomMultiplier { get; set; } = 1;
    }
}
