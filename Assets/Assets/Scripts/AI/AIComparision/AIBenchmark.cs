using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Core;

/// <summary>
/// AIBenchmark — runs N episodes per AI type and prints a comparison table.
///
/// Usage:
///   1. Create a "Benchmark" GameObject in the Scene.
///   2. Attach this script.
///   3. Press Play -> results print to Console (and optional UI Text).
///
/// Note: PPO column uses a greedy heuristic approximation inside this pure-C#
/// simulation because calling the ONNX worker here would require a real scene
/// setup. For a true ONNX benchmark, run the game with AIType switched in
/// BrainEvaluateNode and record results manually.
/// </summary>
public class AIBenchmark : MonoBehaviour
{
    [Header("Episodes per AI type")]
    public int episodesPerAI = 30;
    public int maxStepsPerEpisode = 100;

    [Header("UI (optional)")]
    public TextMeshProUGUI resultText;

    // Lightweight simulation entity (no MonoBehaviour)
    private class SimEntity
    {
        public float hp, maxHp;
        public Vector2 pos;
        public bool IsDead => hp <= 0;
        public float CurrentHealth => hp;
        public float MaxHealth => maxHp;
    }

    private struct EpisodeResult
    {
        public bool win;
        public int steps;
        public float finalHp;
        public int attackCount;
    }

    private struct AISummary
    {
        public string name;
        public float winRate, avgSteps, avgFinalHp, avgAttacks;
    }

    private delegate int AIActionFn(SimEntity self, SimEntity enemy, ref object state);

    private void Start()
    {
        StartCoroutine(RunAll());
    }

    private IEnumerator RunAll()
    {
        var results = new List<AISummary>
        {
            RunBenchmark("PPO (approx)",  SimPPOAction),
            RunBenchmark("Rule-Based",    SimRuleBasedAction),
            RunBenchmark("FSM",           SimFSMAction),
        };
        PrintTable(results);
        yield return null;
    }

    private AISummary RunBenchmark(string aiName, AIActionFn fn)
    {
        var rng = new System.Random(42);
        var results = new List<EpisodeResult>();

        for (int ep = 0; ep < episodesPerAI; ep++)
        {
            var self = new SimEntity { hp = 1f, maxHp = 1f, pos = Vector2.zero };
            var enemy = new SimEntity
            {
                hp = (float)(rng.NextDouble() * 0.8 + 0.2),
                maxHp = 1f,
                pos = RandomEnemyPos(rng)
            };

            object aiState = null;
            int steps = 0, attacks = 0;
            bool win = false;

            for (int s = 0; s < maxStepsPerEpisode; s++)
            {
                steps++;
                int action = fn(self, enemy, ref aiState);
                if (action == 9) attacks++;
                SimStep(action, self, enemy, ref win);
                if (win || self.IsDead) break;
            }

            results.Add(new EpisodeResult
            { win = win, steps = steps, finalHp = self.hp, attackCount = attacks });
        }

        float winRate = 0, avgSteps = 0, avgHp = 0, avgAtk = 0;
        foreach (var r in results)
        {
            if (r.win) winRate++;
            avgSteps += r.steps;
            avgHp += r.finalHp;
            avgAtk += r.attackCount;
        }
        int n = results.Count;
        return new AISummary
        {
            name = aiName,
            winRate = winRate / n * 100f,
            avgSteps = avgSteps / n,
            avgFinalHp = avgHp / n * 100f,
            avgAttacks = avgAtk / n
        };
    }

    // Simulation step — mirrors IsoTacticalEnv.step() logic
    private static readonly Vector2[] DirMap =
    {
        Vector2.zero,
        new Vector2(0,1), new Vector2(0.7f,0.7f),
        new Vector2(1,0), new Vector2(0.7f,-0.7f),
        new Vector2(0,-1), new Vector2(-0.7f,-0.7f),
        new Vector2(-1,0), new Vector2(-0.7f,0.7f)
    };

    private static void SimStep(int action, SimEntity self, SimEntity enemy, ref bool win)
    {
        if (action >= 1 && action <= 8)
            self.pos += DirMap[action] * 0.5f;

        float dist = Vector2.Distance(self.pos, enemy.pos);

        if (action == 9 && dist <= 1.5f)
        {
            enemy.hp -= 0.3f;
            if (enemy.hp <= 0) { win = true; return; }
        }

        if (dist <= 2.0f)
            self.hp -= 0.1f;
    }

    private Vector2 RandomEnemyPos(System.Random rng)
    {
        float angle = (float)(rng.NextDouble() * Mathf.PI * 2);
        float dist = (float)(rng.NextDouble() * 3f + 2f);
        return new Vector2(Mathf.Cos(angle) * dist, Mathf.Sin(angle) * dist);
    }

    // AI decision functions used inside simulation
    private int SimPPOAction(SimEntity self, SimEntity enemy, ref object state)
    {
        // Greedy approximation (real PPO requires ONNX worker in scene mode)
        float dist = Vector2.Distance(self.pos, enemy.pos);
        if (dist <= 1.5f) return 9;
        return GetDirAction((enemy.pos - self.pos).normalized);
    }

    private int SimRuleBasedAction(SimEntity self, SimEntity enemy, ref object state)
    {
        float dist = Vector2.Distance(self.pos, enemy.pos);
        float hpPct = self.hp / self.maxHp;
        if (hpPct < 0.3f) return GetDirAction((self.pos - enemy.pos).normalized);
        if (dist <= 1.5f) return 9;
        return GetDirAction((enemy.pos - self.pos).normalized);
    }

    private enum FSMSt { Chase, Attack, Retreat }

    private int SimFSMAction(SimEntity self, SimEntity enemy, ref object state)
    {
        if (state == null) state = FSMSt.Chase;
        var st = (FSMSt)state;
        float dist = Vector2.Distance(self.pos, enemy.pos);
        float hpPct = self.hp / self.maxHp;

        if (hpPct < 0.3f) st = FSMSt.Retreat;
        else if (dist <= 1.5f) st = FSMSt.Attack;
        else st = FSMSt.Chase;
        state = st;

        return st switch
        {
            FSMSt.Attack => 9,
            FSMSt.Retreat => GetDirAction((self.pos - enemy.pos).normalized),
            _ => GetDirAction((enemy.pos - self.pos).normalized),
        };
    }

    private static int GetDirAction(Vector2 dir)
    {
        dir.Normalize();
        float best = float.MinValue; int idx = 0;
        for (int i = 1; i <= 8; i++)
        {
            float d = Vector2.Dot(dir, DirMap[i]);
            if (d > best) { best = d; idx = i; }
        }
        return idx;
    }

    private void PrintTable(List<AISummary> list)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("==========================================================");
        sb.AppendLine("  AI BENCHMARK RESULTS");
        sb.AppendLine($"  Episodes per AI: {episodesPerAI}   Max steps: {maxStepsPerEpisode}");
        sb.AppendLine("----------------------------------------------------------");
        sb.AppendLine("  AI Type       | Win Rate | Avg Steps | Avg HP% | Avg Atk");
        sb.AppendLine("----------------------------------------------------------");
        foreach (var s in list)
            sb.AppendLine($"  {s.name,-13} | {s.winRate,7:F1}% | {s.avgSteps,9:F1} | " +
                          $"{s.avgFinalHp,6:F1}% | {s.avgAttacks,7:F1}");
        sb.AppendLine("==========================================================");
        sb.AppendLine("  Win Rate  : % of episodes where NPC killed the enemy");
        sb.AppendLine("  Avg Steps : avg steps per episode (fewer = more decisive)");
        sb.AppendLine("  Avg HP %  : avg remaining HP at episode end");
        sb.AppendLine("  Avg Atk   : avg attack count per episode");
        sb.AppendLine();
        sb.AppendLine("  [PPO column uses greedy heuristic — not real ONNX inference.");
        sb.AppendLine("   Switch BrainEvaluateNode.activeAI in Inspector for true test]");

        Debug.Log(sb.ToString());
        if (resultText != null) resultText.text = sb.ToString();
    }
}