using System.Threading.Tasks;
using Content.Server.Doors.Components;
using Content.Shared.Construction;
using JetBrains.Annotations;
using Robust.Shared.GameObjects;
using Robust.Shared.Localization;
using Robust.Shared.Serialization.Manager.Attributes;
using Robust.Shared.Utility;
using static Content.Shared.Doors.SharedDoorComponent;

namespace Content.Server.Construction.Conditions
{
    [UsedImplicitly]
    [DataDefinition]
    public class DoorWelded : IGraphCondition
    {
        [DataField("welded")]
        public bool Welded { get; private set; } = true;

        public async Task<bool> Condition(IEntity entity)
        {
            if (!entity.TryGetComponent(out ServerDoorComponent? doorComponent)) return false;

            return doorComponent.IsWeldedShut == Welded;
        }

        public bool DoExamine(IEntity entity, FormattedMessage message, bool inDetailsRange)
        {
            if (!entity.TryGetComponent(out ServerDoorComponent? door)) return false;

            if (door.IsWeldedShut != Welded)
            {
                if (Welded == true)
                    message.AddMarkup(Loc.GetString("construction-condition-door-weld", ("entityName", entity.Name)) + "\n");
                else
                    message.AddMarkup(Loc.GetString("construction-condition-door-unweld", ("entityName", entity.Name)) + "\n");
                return true;
            }

            return false;
        }
    }
}
