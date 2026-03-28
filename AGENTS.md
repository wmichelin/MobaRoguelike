# MobaRoguelike — Agent & Contributor Architecture Guide

## Layer Separation

### Core (`Assets/Scripts/Core/`, assembly: `MobaRoguelike.Core`)
- Pure C# — **no `using UnityEngine`**, no MonoBehaviours
- Contains: game logic, interfaces, data structures, state machines, pure math
- `noEngineReferences: true` in the `.asmdef`

### Runtime (`Assets/Scripts/Runtime/`, assembly: `MobaRoguelike.Runtime`)
- MonoBehaviours, ScriptableObjects, Unity APIs, physics, rendering
- References Core; **Core never references Runtime**

---

## Adding a New Ability Effect

1. **Choose layer:**
   - Pure math / no Unity types → Core
   - Needs `Physics`, `Transform`, coroutines, VFX → Runtime

2. **Implement `IAbilityEffect`** (`MobaRoguelike.Core.Abilities`):
   ```csharp
   [Serializable]  // System.Serializable — OK in both layers
   public class MyEffect : IAbilityEffect
   {
       public float SomeValue = 1f;
       public void Apply(AbilityContext context) { /* ... */ }
   }
   ```

3. **Mark `[Serializable]`** so Unity's `[SerializeReference]` drawer can persist it in `AbilityDefinitionSO`.

4. If the effect needs to recurse into child `IAbilityEffect` fields (like `CompositeAbilityEffect`), the effect **must live in Runtime** and annotate the child list with `[SerializeReference]`.

5. Assign the effect to an `AbilityDefinitionSO` asset via the `Effect` field in the Inspector.

---

## Compositional / Roguelike Upgrade Pattern

Abilities are built by composing small, single-responsibility `IAbilityEffect` implementations.

### CompositeAbilityEffect
Chains N effects in sequence. Use as the root when stacking upgrades:
```csharp
// AbilityDefinitionSO.Effect = CompositeAbilityEffect
//   └─ SwordSpinEffect  (base)
//   └─ BonusDamageModifier (perk picked in run)
//   └─ SlowOnHitModifier   (another perk)
```

### Decorator / Modifier pattern
Wrap an inner effect to add pre/post behavior:
```csharp
[Serializable]
public class BonusDamageModifier : IAbilityEffect
{
    [SerializeReference] public IAbilityEffect Inner;
    public float BonusDamage = 10f;

    public void Apply(AbilityContext ctx)
    {
        // pre-processing (e.g. boost damage context)
        Inner?.Apply(ctx);
        // post-processing
    }
}
```

### Run-time ability building
At run start, after the player picks perks, build modified `AbilityData` programmatically:
```csharp
var composite = new CompositeAbilityEffect();
composite.Effects.Add(new SwordSpinEffect { Radius = 2f, Damage = 50f });
composite.Effects.Add(new BonusDamageModifier { BonusDamage = 20f });
abilityCaster.SetAbility(AbilitySlot.Q, new AbilityData { Effect = composite, Cooldown = 8f, ... });
```

---

## AbilityContext Fields

```
CasterPositionX / CasterPositionZ   — world position of caster
TargetPositionX / TargetPositionZ   — world position of target (e.g. cursor)
CasterForwardX  / CasterForwardZ    — normalized forward direction of caster
CasterId                            — GameObject.GetInstanceID() of caster
```

---

## IDamageable

Interface in `MobaRoguelike.Core.Abilities`. Implement on any `MonoBehaviour` that can receive damage:
```csharp
public class EnemyHealth : MonoBehaviour, IDamageable
{
    public void TakeDamage(float amount, int sourceId) { /* ... */ }
}
```
`SwordSpinEffect` (and future AoE effects) use `TryGetComponent<IDamageable>()` to apply damage.

---

## Input Bindings

Source of truth: `Assets/InputSystem_Actions.inputactions`

| Action   | Keyboard    | Gamepad         |
|----------|-------------|-----------------|
| Move     | Arrow keys  | Left stick      |
| Dash     | Space       | South button    |
| AbilityQ | Q           | North button    |
| AbilityW | W           | Left shoulder   |
| AbilityE | E           | Right shoulder  |
| AbilityR | R           | Left trigger    |
| Attack   | Left mouse  | West button     |

After editing the `.inputactions` JSON, Unity regenerates `InputSystem_Actions.cs` on import. No manual C# changes needed for binding-only updates.

---

## HUD / Action Bar

- `ActionBarView` — add to any HUD GameObject; builds 4-slot bar procedurally in `Awake`. Assign `_hudController`, `_casterBridge`, and `_font` in Inspector.
- `AbilitySlotView` — owns one slot's visuals (background, key label, ability name, cooldown overlay). Either wired in scene or injected by `ActionBarView.Initialize(...)`.
- `HudController` — subscribes to `AbilityCaster.OnCooldownChanged`; drives `AbilitySlotView.UpdateCooldown`. Call `SetSlotViews(views)` to inject procedurally built views.

---

## Scene Wiring Rules

- New visual components must be **self-constructing** — create their own UI in `Awake` or `Start` using `new GameObject(...)`. Do not require scene prefabs.
- New logic components wire via `GetComponent` / serialized Inspector fields. Use `FindObjectOfType` only as a fallback with a warning log.
- **Never edit `.unity` scene files in code.** All setup flows from component `Awake`/`Start` or Inspector assignments.

---

## Testing

- **EditMode tests** (`Assets/Scripts/Tests/EditMode/`): unit-test Core classes with no scene setup.
- **PlayMode tests** (`Assets/Scripts/Tests/PlayMode/`): integration tests that require a running scene.
- New `IAbilityEffect` implementations should have EditMode tests for their `Apply` logic (mock `AbilityContext`, assert side effects via callbacks or state inspection).
