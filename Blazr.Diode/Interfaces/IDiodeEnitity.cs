/// ============================================================
/// Author: Shaun Curtis, Cold Elm Coders
/// License: Use And Donate
/// If you use it, donate something to a charity somewhere
/// ============================================================

namespace Blazr.Diode;

public interface IDiodeEntity 
{
<<<<<<<< HEAD:Blazr.Diode/Interfaces/IDiodeEnitity.cs
    public Guid Uid { get; }
========
    public record DiodeUid(Guid Uid);
>>>>>>>> b14ca1bd10fafe1b3c278d66b8f3d62c51d12016:Blazr.Diode/Interfaces/DiodeUid.cs
}
