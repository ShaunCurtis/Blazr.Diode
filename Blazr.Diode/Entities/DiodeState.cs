/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode.Entities
{
    public record DiodeState(bool IsNew = true, int StateCode = 0)
    {
        public bool IsMutated { get; private init; }
        public bool IsMarkedForDeletion { get; private init; }

        public DiodeState MarkForDeletion
            => this with { IsMarkedForDeletion = true };
        public DiodeState MarkAsNew
            => this with { IsNew = true };
        public DiodeState Mutated
            => this with { IsMutated = true };
        public DiodeState Persisted
            => this with { IsNew = false, IsMutated = false };

        public static DiodeState New(int StateCode = 0)
            => new DiodeState();

        public static DiodeState Existing(int StateCode = 0)
            => new DiodeState() { IsNew = false };
    }
}
