using MegaCrit.Sts2.Core.Commands;

namespace Riku.RikuCode.Extensions;

public static class AudioHelper
{
    private static readonly Random rng = new Random();
    
    
    private static readonly string[] attackSfx =
    {
        "res://Riku/sounds/riku_atk (1).wav",
        "res://Riku/sounds/riku_atk (2).wav",
        "res://Riku/sounds/riku_atk (3).wav",
        "res://Riku/sounds/riku_atk (4).wav",
        "res://Riku/sounds/riku_atk (5).wav",
        "res://Riku/sounds/riku_atk (6).wav",
    };
    
    private static readonly string[] soraAttackSfx =
    {
        
    };
    
    private static readonly string[] kairiAttackSfx =
    {
        
    };
    
    private static readonly string[] finalAttackSfx =
    {
        "res://Riku/sounds/riku_finisher (1).wav",
        "res://Riku/sounds/riku_finisher (2).wav",
        "res://Riku/sounds/riku_finisher (3).wav",
        "res://Riku/sounds/riku_finisher (4).wav",
        "res://Riku/sounds/riku_finisher (5).wav",
        "res://Riku/sounds/riku_finisher (6).wav",
        "res://Riku/sounds/riku_finisher (7).wav",
        "res://Riku/sounds/riku_finisher (8).wav",
        "res://Riku/sounds/riku_finisher (9).wav",
        "res://Riku/sounds/riku_finisher (10).wav",
        "res://Riku/sounds/riku_finisher (11).wav",
    };

    private static readonly string[] callSoraSfx =
    {
        "res://Sora/sounds/riku_1.wav",
        "res://Sora/sounds/riku_2.wav",
    };
    
    private static readonly string[] trueEndSfx =
    {
        "res://Sora/sounds/trinity_limit.wav",
        "res://Sora/sounds/trinity_limit_2.wav",
    };
    
    private static readonly string[] formchangeSfx =
    {
        "res://Sora/sounds/formchange_1.wav",
        "res://Sora/sounds/formchange_2.wav",
    };
    
    private static readonly string[] damagedSfx =
    {
        "res://Riku/sounds/hit_normal (1).wav",
        "res://Riku/sounds/hit_normal (2).wav",
        "res://Riku/sounds/hit_normal (3).wav",
        "res://Riku/sounds/hit_normal (4).wav",
    };
    
    private static readonly string[] highDamagedSfx =
    {
        "res://Riku/sounds/hit_high (1).wav",
        "res://Riku/sounds/hit_high (2).wav",
        "res://Riku/sounds/hit_high (3).wav",
        
    };
    
    private static readonly string[] criticalDamagedSfx =
    {
        "res://Riku/sounds/hit_critical (1).wav",
        "res://Riku/sounds/hit_critical (2).wav",
        "res://Riku/sounds/hit_critical (3).wav",
    };

    private static readonly string[] fireSfx =
    {
        "res://Riku/sounds/dark_firaga.wav",
        "res://Riku/sounds/dark_firaga_2.wav",
    };

    private static readonly string[] victorySfx =
    {
        "res://Riku/sounds/victory_1.wav",
        "res://Riku/sounds/victory_2.wav",
        "res://Riku/sounds/victory_3.wav",
    };
    
    private static readonly string[] gameoverSfx =
    {
        "res://Riku/sounds/gameover_1.wav",
        "res://Riku/sounds/gameover_2.wav",
    };
    
    public static void PlayRandomAttack()
    {
        PlayRandom(attackSfx);
    }
    
    public static void PlayRandomFinalAttack()
    {
        PlayRandom(finalAttackSfx);
    }

    public static void PlayRandomTrueEnd()
    {
        PlayRandom(trueEndSfx);
    }
    
    public static void PlayRandomGameover()
    {
        PlayRandom(gameoverSfx);
    }

    public static void PlayRandomSora()
    {
        PlayRandom(callSoraSfx);
    }

    public static void PlayRandomSoraAtk()
    {
        PlayRandom(soraAttackSfx);
    }
    
    public static void PlayRandomDamaged()
    {
        PlayRandom(damagedSfx);
    }
    
    public static void PlayRandomDamagedHigh()
    {
        PlayRandom(highDamagedSfx);
    }
    
    public static void PlayRandomDamagedCritical()
    {
        PlayRandom(criticalDamagedSfx);
    }
    
    public static void PlayRandomFire()
    {
        PlayRandom(fireSfx);
    }

    public static void PlayRandomVictory()
    {
        PlayRandom(victorySfx);
    }

    public static void PlayRandom(string[] pool)
    {
        int index = rng.Next(pool.Length);
        SfxCmd.Play(pool[index]);
    }
}