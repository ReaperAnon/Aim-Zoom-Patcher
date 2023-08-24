using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.FormKeys.Fallout4;

namespace AimZoomPatcher
{
    public class Program
    {
        public static Lazy<ModSettings> _config = null!;

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<IFallout4Mod, IFallout4ModGetter>(RunPatch)
                .SetAutogeneratedSettings("settings", "settings.json", out _config)
                .SetTypicalOpen(GameRelease.Fallout4, "YourPatcher.esp")
                .Run(args);
        }

        public static bool IsReflexSight(IWeaponModificationGetter weaponModGetter)
        {
            return weaponModGetter.Properties.Any(entry => entry is IObjectModFormLinkIntPropertyGetter<Weapon.Property> formGetter && formGetter.Record.Equals(Fallout4.Keyword.dn_HasScope_ReflexSight)) ||
                   weaponModGetter.Properties.Any(entry => entry is IObjectModFormLinkFloatPropertyGetter<Weapon.Property> formGetter && formGetter.Record.Equals(Fallout4.Keyword.dn_HasScope_ReflexSight));
        }

        public static void RunPatch(IPatcherState<IFallout4Mod, IFallout4ModGetter> state)
        {
            foreach (var objModGetter in state.LoadOrder.PriorityOrder.AObjectModification().WinningOverrides())
            {
                if (_config.Value.ReflexSightZoomMultiplier == 0)
                    break;

                if (objModGetter is not IWeaponModificationGetter weaponModGetter)
                    continue;

                if (!_config.Value.AffectsScopes && !IsReflexSight(weaponModGetter))
                    continue;

                try
                {
                    var weaponModSetter = weaponModGetter.DeepCopy();
                    var zoomProp = weaponModSetter.Properties.First(entry => entry is ObjectModFloatProperty<Weapon.Property> floatProp && floatProp.Property == Weapon.Property.ZoomDataFOVMult) as ObjectModFloatProperty<Weapon.Property>;
                    zoomProp!.Value = _config.Value.ReflexSightZoomMultiplier > 0 ? _config.Value.ReflexSightZoomMultiplier : (zoomProp.Value - 1) * (1 + _config.Value.ReflexSightZoomMultiplier) + 1;
                    state.PatchMod.ObjectModifications.Set(weaponModSetter);
                }
                catch (InvalidOperationException ex) { }
            }

            foreach (var zoomGetter in state.LoadOrder.PriorityOrder.Zoom().WinningOverrides())
            {
                if (_config.Value.IronSightsZoomMultiplier < 1)
                    break;

                if (zoomGetter.FovMult != 0)
                    continue;

                var zoomSetter = state.PatchMod.Zooms.GetOrAddAsOverride(zoomGetter);
                zoomSetter.FovMult = _config.Value.IronSightsZoomMultiplier == 1 ? 0.999999f : _config.Value.IronSightsZoomMultiplier;
            }
        }
    }
}