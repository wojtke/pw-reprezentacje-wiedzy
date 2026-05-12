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
