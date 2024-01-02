using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;

namespace TemplatePlugin;

[MinimumApiVersion(129)]
public class TemplatePlugin : BasePlugin
{
    private const string Version = "1.0.0";
    
    public override string ModuleName => "{PLUGIN_NAME} Plugin";
    public override string ModuleVersion => Version;
    public override string ModuleAuthor => "{PLUGIN_AUTHOR}";
    public override string ModuleDescription => "{PLUGIN_DESCRIPTION}";
    
    public static readonly string LogPrefix = $"[PLUGIN_NAME {Version}] ";
    public static readonly string MessagePrefix = $"[{ChatColors.Green}PLUGIN_NAME{ChatColors.White}] ";
    
    [GameEventHandler]
    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        Console.WriteLine($"{LogPrefix}OnRoundStart event fired!");
        Server.PrintToChatAll($"{MessagePrefix}OnRoundStart event fired!");

        return HookResult.Continue;
    }
}
