// Polyfill necessário para 'init' setters (C# 9+) ao compilar para .NET Framework.
namespace System.Runtime.CompilerServices
{
    internal sealed class IsExternalInit { }
}
