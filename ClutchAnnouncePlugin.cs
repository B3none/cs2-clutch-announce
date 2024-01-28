using ClutchAnnouncePlugin.Modules;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Utils;

namespace ClutchAnnouncePlugin;

[MinimumApiVersion(147)]
public class ClutchAnnouncePlugin : BasePlugin
{
    private const string Version = "1.1.0";
    
    public override string ModuleName => "Clutch Announce Plugin";
    public override string ModuleVersion => Version;
    public override string ModuleAuthor => "B3none";
    public override string ModuleDescription => "Announce when someone wins a clutch scenario";

    // Constants
    private const string LogPrefix = $"[Clutch Announce {Version}] ";
    private static string MessagePrefix = $"[{ChatColors.Green}Retakes{ChatColors.White}] ";
    
    // Config
    private const int MinPlayers = 3;
    private Translator _translator;
    
    // State
    private int _opponents;
    private CsTeam? _clutchTeam;
    private CCSPlayerController? _clutchPlayer;
    
    public ClutchAnnouncePlugin()
    {
        _translator = new Translator(Localizer);
    }

    public override void Load(bool hotReload)
    {
        _translator = new Translator(Localizer);

        MessagePrefix = _translator["clutch_announce.prefix"];
    }

    // Listeners
    [GameEventHandler]
    public HookResult OnClientDisconnect(EventClientDisconnect @event, GameEventInfo info)
    {
        CheckAlive();

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        CheckAlive();

        return HookResult.Continue;
    }

    [GameEventHandler]
    public HookResult OnRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        Console.WriteLine($"{LogPrefix}OnRoundEnd event fired!");

        if (_clutchPlayer != null && IsValidPlayer(_clutchPlayer) && (CsTeam)@event.Winner == _clutchTeam)
        {
            Server.PrintToChatAll(MessagePrefix + _translator["clutch_announce.clutched", _clutchPlayer.PlayerName, _opponents]);
        }
        
        _clutchPlayer = null;
        _clutchTeam = null;
        _opponents = 0;

        return HookResult.Continue;
    }
    
    // Utilities
    private void CheckAlive()
    {
        if (_clutchPlayer != null)
        {
            return;
        }
        
        List<CCSPlayerController> aliveCts = new();
        List<CCSPlayerController> aliveTs = new();

        var players = Utilities.GetPlayers().Where(IsValidPlayer).ToList();
        
        foreach (var player in players)
        {
            if (player.Team == CsTeam.CounterTerrorist && player.PlayerPawn.Value!.Health > 0)
            {
                aliveCts.Add(player);
            }
            else if (player.Team == CsTeam.Terrorist && player.PlayerPawn.Value!.Health > 0)
            {
                aliveTs.Add(player);
            }
        }

        if (aliveTs.Count == 1 && aliveCts.Count >= MinPlayers)
        {
            _clutchTeam = CsTeam.Terrorist;
            _opponents = aliveCts.Count;

            _clutchPlayer = aliveTs.First();
        }
        else if (aliveCts.Count == 1 && aliveTs.Count >= MinPlayers)
        {
            _clutchTeam = CsTeam.CounterTerrorist;
            _opponents = aliveTs.Count;
            
            _clutchPlayer = aliveCts.First();
        }
    }
    
    // Helpers
    private static bool IsValidPlayer(CCSPlayerController player)
    {
        return player.IsValid 
           && player.PlayerPawn.IsValid
           && player.PlayerPawn.Value != null
           && player.PlayerPawn.Value.IsValid;
    }
}
