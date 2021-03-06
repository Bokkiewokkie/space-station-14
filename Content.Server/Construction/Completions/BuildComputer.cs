using System.Linq;
using System.Threading.Tasks;
using Content.Server.Construction.Components;
using Content.Shared.Construction;
using JetBrains.Annotations;
using Robust.Shared.Containers;
using Robust.Shared.GameObjects;
using Robust.Shared.Log;
using Robust.Shared.Serialization.Manager.Attributes;

namespace Content.Server.Construction.Completions
{
    [UsedImplicitly]
    [DataDefinition]
    public class BuildComputer : IGraphAction
    {
        [DataField("container")] public string Container { get; private set; } = string.Empty;

        public async Task PerformAction(IEntity entity, IEntity? user)
        {
            if (!entity.TryGetComponent(out ContainerManagerComponent? containerManager))
            {
                Logger.Warning($"Computer entity {entity} did not have a container manager! Aborting build computer action.");
                return;
            }

            if (!containerManager.TryGetContainer(Container, out var container))
            {
                Logger.Warning($"Computer entity {entity} did not have the specified '{Container}' container! Aborting build computer action.");
                return;
            }

            if (container.ContainedEntities.Count != 1)
            {
                Logger.Warning($"Computer entity {entity} did not have exactly one item in the specified '{Container}' container! Aborting build computer action.");
            }

            var board = container.ContainedEntities[0];

            if (!board.TryGetComponent(out ComputerBoardComponent? boardComponent))
            {
                Logger.Warning($"Computer entity {entity} had an invalid entity in container \"{Container}\"! Aborting build computer action.");
                return;
            }

            var entityManager = entity.EntityManager;
            container.Remove(board);

            var computer = entityManager.SpawnEntity(boardComponent.Prototype, entity.Transform.Coordinates);
            computer.Transform.LocalRotation = entity.Transform.LocalRotation;

            var computerContainer = ContainerHelpers.EnsureContainer<Container>(computer, Container, out var existed);

            if (existed)
            {
                // In case there are any entities inside this, delete them.
                foreach (var ent in computerContainer.ContainedEntities.ToArray())
                {
                    computerContainer.ForceRemove(ent);
                    ent.Delete();
                }
            }

            computerContainer.Insert(board);

            if (computer.TryGetComponent(out ConstructionComponent? construction))
            {
                // We only add this container. If some construction needs to take other containers into account, fix this.
                construction.AddContainer(Container);
            }

            entity.Delete();
        }
    }
}
