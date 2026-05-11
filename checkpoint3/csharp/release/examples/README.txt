Examples folder

Każdy przykład składa się z trzech plików:
- <id>.domain - dziedzina DS4
- <id>.query - pojedyncza kwerenda
- <id>.expected - oczekiwana odpowiedź TAK albo NIE

GUI ładuje pary .domain i .query z tego folderu. Pliki .expected są używane przez testy C# w xUnit.

Liczba przykładów: 40
TAK: 23
NIE: 17

Przykłady z prefiksem tex pochodzą z checkpoint2_v1.tex. Przykłady z prefiksem extra są dodatkowymi przypadkami testowymi.

Lista:
- tak_tex_01_ysp_possibly_executable: TAK - TAK TEX 01 - YSP possibly executable
- nie_tex_02_ysp_necessary_executable: NIE - NIE TEX 02 - YSP necessary executable
- tak_tex_03_ysp_possibly_dead: TAK - TAK TEX 03 - YSP possibly not alive
- nie_tex_04_ysp_necessary_dead: NIE - NIE TEX 04 - YSP necessary not alive
- tak_tex_05_producer_necessary_executable: TAK - TAK TEX 05 - Producent necessary executable
- tak_tex_06_producer_possibly_empty: TAK - TAK TEX 06 - Producent possibly empty
- nie_tex_07_producer_necessary_empty: NIE - NIE TEX 07 - Producent necessary empty
- tak_tex_08_producer_no_products: TAK - TAK TEX 08 - Producent no products
- tak_tex_09_switches_necessary_executable: TAK - TAK TEX 09 - Przełączniki necessary executable
- tak_tex_10_switches_possibly_executable: TAK - TAK TEX 10 - Przełączniki possibly executable
- nie_tex_11_switches_necessary_alarm: NIE - NIE TEX 11 - Przełączniki necessary alarm
- tak_tex_12_switches_possibly_alarm: TAK - TAK TEX 12 - Przełączniki possibly alarm
- nie_tex_13_workers_p1_necessary_executable: NIE - NIE TEX 13 - Robotnicy P1 necessary executable
- tak_tex_14_workers_p1_possibly_executable: TAK - TAK TEX 14 - Robotnicy P1 possibly executable
- nie_tex_15_workers_p1_necessary_outside: NIE - NIE TEX 15 - Robotnicy P1 necessary outside
- tak_tex_16_workers_p1_possibly_outside: TAK - TAK TEX 16 - Robotnicy P1 possibly outside
- tak_tex_17_workers_p2_necessary_executable: TAK - TAK TEX 17 - Robotnicy P2 necessary executable
- tak_tex_18_workers_p2_possibly_executable: TAK - TAK TEX 18 - Robotnicy P2 possibly executable
- nie_tex_19_workers_p2_necessary_outside: NIE - NIE TEX 19 - Robotnicy P2 necessary outside
- tak_tex_20_workers_p2_possibly_outside: TAK - TAK TEX 20 - Robotnicy P2 possibly outside
- tak_extra_01_simple_cause: TAK - TAK EXTRA 01 - Prosty efekt
- tak_extra_02_possible_release: TAK - TAK EXTRA 02 - Możliwy efekt releases
- tak_extra_03_noop_executable: TAK - TAK EXTRA 03 - Akcja bez reguł wykonalna
- tak_extra_04_ramification: TAK - TAK EXTRA 04 - Ramifikacja always
- tak_extra_05_noninertial_alarm: TAK - TAK EXTRA 05 - Alarm nieinercyjny
- tak_extra_06_composite_no_conflict: TAK - TAK EXTRA 06 - Akcja złożona bez konfliktu
- tak_extra_07_conflict_possible_p: TAK - TAK EXTRA 07 - Konflikt i możliwe p
- tak_extra_08_partial_initial_possible: TAK - TAK EXTRA 08 - Częściowy stan początkowy
- tak_extra_09_impossible_not_active: TAK - TAK EXTRA 09 - Impossible nieaktywne
- tak_extra_10_two_step_process: TAK - TAK EXTRA 10 - Proces dwuetapowy
- nie_extra_01_release_not_necessary: NIE - NIE EXTRA 01 - Releases nie daje konieczności
- nie_extra_02_impossible_open: NIE - NIE EXTRA 02 - Akcja zablokowana
- nie_extra_03_noop_goal_false: NIE - NIE EXTRA 03 - Brak efektu celu
- nie_extra_04_conflict_not_necessary: NIE - NIE EXTRA 04 - Konflikt nie daje konieczności
- nie_extra_05_missing_condition: NIE - NIE EXTRA 05 - Warunek efektu niespełniony
- nie_extra_06_always_blocks_action: NIE - NIE EXTRA 06 - Always blokuje efekt
- nie_extra_07_partial_necessary_executable: NIE - NIE EXTRA 07 - Częściowy start i blokada
- nie_extra_08_two_steps_undo_goal: NIE - NIE EXTRA 08 - Drugi krok cofa cel
- nie_extra_09_second_step_blocked: NIE - NIE EXTRA 09 - Blokada po pierwszym kroku
- nie_extra_10_partial_initial_not_necessary: NIE - NIE EXTRA 10 - Częściowy start i necessary
