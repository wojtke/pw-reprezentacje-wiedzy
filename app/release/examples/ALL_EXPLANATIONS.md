# Zbiorcze wyjaśnienia przykładów

# Demo 01 - warunkowy efekt i possibly

## Cel przykładu

Ten przykład pokazuje działanie reguły:

```text
action causes q if p
```

Czyli akcja `action` powoduje `q`, ale tylko wtedy, gdy przed wykonaniem akcji prawdziwe jest `p`.

## Domena

```text
fluents p, q
actions action
initially p and !q
action causes q if p
```

Mamy dwa fluenty: `p` i `q`. Warunek początkowy mówi, że `p` jest prawdziwe, a `q` fałszywe, więc jest dokładnie jeden stan początkowy:

```text
{p, ¬q}
```

## Kwerenda

```text
possibly q after action
```

Kwerenda `possibly` jest prawdziwa, gdy z każdego stanu początkowego istnieje przynajmniej jedna pełna ścieżka kończąca się stanem spełniającym cel.

## Przebieg

W jedynym stanie początkowym `{p, ¬q}` warunek `p` zachodzi. Akcja wymusza `q`, więc wynik to:

```text
{p, ¬q} -> {p, q}
```

## Odpowiedź

Odpowiedź powinna być:

```text
TAK
```

Z jedynego stanu początkowego istnieje ścieżka prowadząca do stanu z `q`.

## Uwaga o opisie częściowym

Gdyby warunek początkowy mówił tylko `initially !q` (nic o `p`), stanami początkowymi byłyby `{p, ¬q}` i `{¬p, ¬q}`. Ze stanu `{¬p, ¬q}` żadna ścieżka nie prowadzi do `q`, więc przy semantyce "dla każdego stanu początkowego istnieje ścieżka" odpowiedź brzmiałaby NIE.

# Demo 02 - warunkowy efekt i necessary

## Cel przykładu

Ten przykład pokazuje różnicę między `possibly` i `necessary`.

Domena jest taka sama jak w Demo 01:

```text
fluents p, q
actions action
initially !q
action causes q if p
```

## Stany początkowe

Warunek `initially !q` ustala tylko wartość `q`. Fluent `p` nie jest określony, więc możliwe są dwa stany początkowe:

```text
{¬p, ¬q}
{p, ¬q}
```

## Kwerenda

```text
necessary q after action
```

Pytamy, czy po wykonaniu akcji `action` na każdej możliwej ścieżce `q` będzie prawdziwe.

## Przebieg

Gałąź pierwsza:

```text
{¬p, ¬q} -> {¬p, ¬q}
```

Tutaj warunek `p` nie zachodzi, więc `q` nie zostaje ustawione.

Gałąź druga:

```text
{p, ¬q} -> {p, q}
```

Tutaj warunek `p` zachodzi, więc akcja ustawia `q`.

## Odpowiedź

Odpowiedź powinna być:

```text
NIE
```

`q` jest prawdziwe tylko w jednej z możliwych gałęzi. Kwerenda `necessary` wymaga, żeby warunek był spełniony we wszystkich gałęziach, więc wynik jest fałszywy.


---

# Demo 03 - niedeterminizm przez releases i possibly

## Cel przykładu

Ten przykład pokazuje regułę `releases`, czyli zwolnienie fluentu spod zwykłej inercji.

## Domena

```text
fluents loaded, alive
actions spin, shoot, load
initially alive
spin releases loaded if true
shoot causes !alive if loaded
impossible load if loaded
load causes loaded if !loaded
```

Fluent `alive` oznacza, że postać żyje. Fluent `loaded` oznacza, że broń jest załadowana.

Reguła:

```text
spin releases loaded if true
```

mówi, że po akcji `spin` wartość `loaded` może się zmienić niedeterministycznie. Reasoner nie musi zachować poprzedniej wartości `loaded`.

Reguła:

```text
shoot causes !alive if loaded
```

mówi, że strzał zabija tylko wtedy, gdy broń jest załadowana.

## Kwerenda

```text
possibly !alive after spin; shoot
```

Pytamy, czy z każdego stanu początkowego istnieje ścieżka, w której po wykonaniu `spin`, a potem `shoot`, postać nie żyje.

## Przebieg

Po `spin` fluent `loaded` jest zwolniony. To znaczy, że możliwy jest stan, w którym `loaded` jest prawdziwe, oraz stan, w którym `loaded` jest fałszywe.

Jeśli po `spin` mamy:

```text
loaded = true
```

wtedy `shoot` powoduje:

```text
!alive
```

Jeśli po `spin` mamy:

```text
loaded = false
```

wtedy `shoot` nie zabija.

## Odpowiedź

Odpowiedź powinna być:

```text
TAK
```

Z każdego stanu początkowego istnieje ścieżka, na której broń jest załadowana po `spin`, a potem `shoot` powoduje `!alive`.


---

# Demo 04 - releases i necessary

## Cel przykładu

Ten przykład pokazuje, że `possibly` i `necessary` mogą dawać różne odpowiedzi w domenach niedeterministycznych.

## Domena

Domena jest taka sama jak w Demo 03:

```text
fluents loaded, alive
actions spin, shoot, load
initially alive
spin releases loaded if true
shoot causes !alive if loaded
impossible load if loaded
load causes loaded if !loaded
```

## Kwerenda

```text
necessary !alive after spin; shoot
```

Pytamy, czy po wykonaniu `spin`, a potem `shoot`, postać będzie martwa na każdej możliwej ścieżce.

## Przebieg

Po `spin` fluent `loaded` jest zwolniony. To daje co najmniej dwie intuicyjne możliwości:

```text
loaded = true
loaded = false
```

Jeśli `loaded = true`, wtedy `shoot` powoduje `!alive`.

Jeśli `loaded = false`, wtedy `shoot` nie ma aktywnego efektu zabicia, więc `alive` zostaje zachowane przez inercję.

## Odpowiedź

Odpowiedź powinna być:

```text
NIE
```

Nie wszystkie ścieżki prowadzą do `!alive`. Wystarczy jedna ścieżka, na której broń nie jest załadowana i postać przeżywa, żeby `necessary !alive` było fałszywe.


---

# Demo 05 - impossible i executable

## Cel przykładu

Ten przykład pokazuje różnicę między akcją, która nic nie zmienia, a akcją, której nie wolno wykonać.

## Domena

```text
fluents loaded
actions load
initially loaded
impossible load if loaded
load causes loaded if !loaded
```

Stan początkowy wymusza:

```text
loaded = true
```

Reguła:

```text
impossible load if loaded
```

mówi, że akcji `load` nie można wykonać, jeśli `loaded` jest już prawdziwe.

## Kwerenda

```text
possibly executable after load
```

Pytamy, czy z każdego stanu początkowego istnieje pełna ścieżka, na której proces składający się z akcji `load` jest wykonywalny.

## Przebieg

Jedyny stan początkowy to:

```text
{loaded}
```

W tym stanie zachodzi warunek `loaded`, więc reguła `impossible load if loaded` blokuje akcję `load`.

Nie ma żadnego stanu następnego.

## Odpowiedź

Odpowiedź powinna być:

```text
NIE
```

Proces nie jest możliwy do wykonania, ponieważ akcja `load` jest zablokowana już w stanie początkowym.


---

# Demo 06 - konflikt akcji złożonych i possibly

## Cel przykładu

Ten przykład pokazuje akcję złożoną oraz konflikt między akcjami.

## Domena

```text
fluents p
actions make_p, make_not_p
initially !p
make_p causes p if true
make_not_p causes !p if true
```

Akcja `make_p` próbuje ustawić `p` na prawdę.

Akcja `make_not_p` próbuje ustawić `p` na fałsz.

## Kwerenda

```text
possibly p after {make_p,make_not_p}
```

Pytamy, czy po wykonaniu złożonego kroku `{make_p,make_not_p}` możliwe jest, że `p` będzie prawdziwe.

## Konflikt

Krok:

```text
{make_p,make_not_p}
```

oznacza próbę równoległego wykonania obu akcji.

Akcje są jednak w konflikcie, bo jedna wymusza:

```text
p
```

a druga wymusza:

```text
!p
```

Reasoner nie wykonuje sprzecznych efektów naraz. Zamiast tego rozważa maksymalne bezkonfliktowe dekompozycje, czyli tutaj:

```text
{make_p}
{make_not_p}
```

## Przebieg

Po dekompozycji `{make_p}` dostajemy:

```text
{p}
```

Po dekompozycji `{make_not_p}` dostajemy:

```text
{¬p}
```

## Odpowiedź

Odpowiedź powinna być:

```text
TAK
```

Istnieje dekompozycja prowadząca do stanu z `p`, więc kwerenda `possibly p` jest prawdziwa.


---

# Demo 07 - konflikt akcji złożonych i necessary

## Cel przykładu

Ten przykład używa tej samej domeny co Demo 06, ale pokazuje odpowiedź dla kwerendy `necessary`.

## Domena

```text
fluents p
actions make_p, make_not_p
initially !p
make_p causes p if true
make_not_p causes !p if true
```

## Kwerenda

```text
necessary p after {make_p,make_not_p}
```

Pytamy, czy po wykonaniu kroku złożonego `{make_p,make_not_p}` fluent `p` jest prawdziwy na każdej możliwej ścieżce.

## Dekompozycje

Ponieważ akcje są w konflikcie, reasoner rozważa maksymalne bezkonfliktowe dekompozycje:

```text
{make_p}
{make_not_p}
```

Pierwsza prowadzi do:

```text
{p}
```

Druga prowadzi do:

```text
{¬p}
```

## Odpowiedź

Odpowiedź powinna być:

```text
NIE
```

Nie każda możliwa dekompozycja prowadzi do `p`. Kwerenda `necessary p` jest więc fałszywa.


---

# Demo 08 - always, noninertial i konsekwencje stanu

## Cel przykładu

Ten przykład pokazuje ograniczenia `always` oraz fluenty `noninertial`.

## Domena

```text
fluents s1, s2, light, alarm
actions toggle1, toggle2
always (s1 or s2) -> light
always light -> alarm
noninertial light
noninertial alarm
initially !s1 and !s2
toggle1 causes s1 if !s1
toggle1 causes !s1 if s1
toggle2 causes s2 if !s2
toggle2 causes !s2 if s2
```

Fluenty `s1` i `s2` oznaczają przełączniki.

Fluent `light` oznacza światło.

Fluent `alarm` oznacza alarm.

## Reguły always

Pierwsza reguła:

```text
always (s1 or s2) -> light
```

mówi, że jeśli którykolwiek przełącznik jest włączony, to światło musi być włączone.

Druga reguła:

```text
always light -> alarm
```

mówi, że jeśli światło jest włączone, to alarm musi być włączony.

## Noninertial

Reguły:

```text
noninertial light
noninertial alarm
```

oznaczają, że `light` i `alarm` nie są zwykłymi fluentami pamiętającymi poprzednią wartość przez inercję. Ich wartość jest traktowana bardziej jak konsekwencja ograniczeń stanu.

## Kwerenda

```text
necessary alarm after {toggle1,toggle2}
```

Pytamy, czy po równoległym wykonaniu `toggle1` i `toggle2` alarm jest koniecznie prawdziwy.

## Przebieg

Początkowo:

```text
s1 = false
s2 = false
```

Akcja `toggle1` ustawia `s1`, a akcja `toggle2` ustawia `s2`. Po kroku złożonym mamy więc włączony co najmniej jeden przełącznik, w praktyce oba:

```text
s1 = true
s2 = true
```

Z reguły `always (s1 or s2) -> light` wynika, że `light` musi być prawdziwe.

Z reguły `always light -> alarm` wynika, że `alarm` musi być prawdziwe.

## Odpowiedź

Odpowiedź powinna być:

```text
TAK
```

Każdy legalny stan końcowy po tym kroku musi spełniać `alarm`.


---
