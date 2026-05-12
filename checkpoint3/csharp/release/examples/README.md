# Przykłady demo po poprawce semantyki possibly

Ten folder zawiera przykłady `.domain`, `.query`, `.expected` oraz `.explanation.md`.

Najważniejsza poprawka:

```text
possibly goal after process
```

jest interpretowane jako:

```text
dla każdego stanu początkowego istnieje pełna ścieżka wykonania procesu, która kończy się stanem spełniającym goal
```

Analogicznie:

```text
possibly executable after process
```

znaczy:

```text
z każdego stanu początkowego istnieje co najmniej jedna pełna ścieżka wykonania procesu
```

To nie jest już błędne, globalne `istnieje jeden stan początkowy i jedna dobra ścieżka`.

Lista przykładów:

- `demo_01_context_keyword_action_possibly_q_false` - NIE - Demo 01 - action jako nazwa akcji, possibly q daje NIE
- `demo_02_context_keyword_action_possibly_q_true_when_p_initial` - TAK - Demo 02 - action jako nazwa akcji, possibly q daje TAK przy initially p
- `demo_03_possibly_all_initial_states_set_q_true` - TAK - Demo 03 - possibly true, bo akcja ustawia q z każdego stanu początkowego
- `demo_04_query_only_fluent_epsilon_possibly_p_false` - NIE - Demo 04 - fluent tylko z kwerendy, possibly p after epsilon daje NIE
- `demo_05_query_only_fluent_epsilon_possibly_p_true_with_initially_p` - TAK - Demo 05 - possibly p after epsilon daje TAK, gdy initially p
- `demo_06_action_inferred_from_query_noop_true` - TAK - Demo 06 - akcja z kwerendy bez reguł jest no-op
- `demo_07_possibly_executable_partial_block_false` - NIE - Demo 07 - possibly executable daje NIE, gdy jakiś stan początkowy blokuje proces
- `demo_08_possibly_release_true_from_every_initial_state` - TAK - Demo 08 - releases daje possibly TAK z każdego stanu początkowego
- `demo_09_keyword_like_fluent_action_name_true` - TAK - Demo 09 - fluent jako nazwa akcji
- `demo_10_explicit_and_inferred_symbols_mix_true` - TAK - Demo 10 - mieszanka deklaracji jawnych i symboli z kontekstu
