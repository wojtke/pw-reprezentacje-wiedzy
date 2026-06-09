# Feedback po Checkpoint 2 – wersja skonsolidowana

Zebrane uwagi z dwóch źródeł: notatki prowadzącego (`feedback-after-checkpoint-2-submit.md`)
oraz notatki własne. Punkty pokrywające się scalone, sprzeczne oznaczone jako **DO USTALENIA**.

---

## 1. Reorganizacja struktury dokumentu (najważniejsze)

Totalna reorganizacja wokół **dwóch osobnych języków**:

- **Język opisu akcji (typu AR)** — tylko **akcje proste**.
  Zdefiniować: **syntaktykę** i **semantykę** (modele), i na tym zamknąć tę sekcję.
- **Język kwerend** — osobna sekcja, z własną **syntaktyką** i **semantyką (drugi model, AC)**.
  Tutaj trafiają: **akcje złożone**, funkcja `res` (i `res_0`), dekompozycje akcji
  oraz semantyka procesów. To **nie** należy do języka akcji.

Powiązane porządki:
- **Mniej sekcji.** Nie tworzyć sekcji/podsekcji bez żadnego tekstu — każda musi mieć
  choćby krótki opis tego, co zawiera.

## 2. Podstawy formalne i decyzje projektowe

- **Wywalić „podstawy formalne”.** To, co potrzebne (prawdopodobnie tylko **definicja formuł**),
  przerzucić do odpowiednich sekcji.
- **Wywalić sekcję „decyzje projektowe”** — opisać te decyzje w sekcjach, których dotyczą.
- **Treści algorytmiczne / implementacyjne przenieść do części technicznej** (następny dokument /
  raport końcowy). Część teoretyczna definiuje język formalnie; implementacja to osobna rzecz.

## 3. Konstrukcje języka

- **`impossible` i `initial` to przypadki szczególne `after` i `causes`**, a nie osobne typy zdań.
  Zapisać to **raz** w syntaktyce (jako skróty/instancje), a dalej mówić już tylko o `after` / `causes`.
- **Funkcja `res` musi być poprawnie zadeklarowana** — podać **dziedzinę i przeciwdziedzinę**
  zanim się jej użyje. To samo dotyczy `res_0`. Obecnie pojawia się znikąd.
- **Zdefiniować wszystkie oznaczenia symbolicznie** — zwłaszcza dziedziny i wartości funkcji,
  a także wszystkie zbiory.

## 4. Semantyka `necessary` / `possible` — **DO USTALENIA**

Wiele stanów początkowych ⇒ jeśli `σ₀` jest *zbiorem* stanów początkowych, to mamy **wiele modeli**;
semantyka musi to odzwierciedlać. Co do operatora `possible` źródła są rozbieżne:

- **Wersja z notatek własnych:** `possibly` = *(dla **każdego** stanu początkowego, dla **jakiejś**
  ścieżki)* — wyraźnie **nie** „dla jakiegokolwiek stanu początkowego”.
- **Wersja z notatki prowadzącego (dosłownie):** `possible` = „**istnieje** stan początkowy ze ścieżką”
  — ale ta sama notatka zaraz dodaje, że interpretacja „istnieje stan początkowy” jest **błędna**.

> Obie notatki zgadzają się, że odczyt „istnieje stan początkowy” jest **zły**. Najpewniej
> wersja własna jest poprawna, a w notatce prowadzącego to przejęzyczenie/błąd zapisu.
> **Zostawione jako otwarte — potwierdzić z prowadzącym / zespołem.**

Dla porównania, `necessary` (bezsporne): zachodzi w **każdym** modelu i dla **każdego** stanu
początkowego (dla każdej ścieżki).

## 5. Styl pisania

- **Mniej symboli, więcej słów.** Raczej nie pisać `∀` w zdaniach — opisywać je tekstem.
- **Dodać konkretne przykłady** w całym dokumencie. Formalne definicje bez przykładów są trudne
  w odbiorze; jako wzór podano scenariusz kowbojów / pojedynku oraz amerykańskie podręczniki CS
  (czytelne pisanie techniczne).
- **Nie kopiować surowej notacji z wykładu bez wyjaśnienia.** Każde formalne wyrażenie objaśnić
  w kontekście.

---

## 6. Sprawy administracyjne (z notatki prowadzącego)

- **Nieobecny członek zespołu (Sebastian)** musi: (a) dostarczyć formalne usprawiedliwienie
  (zwolnienie lekarskie, pismo z dziekanatu itp.), oraz (b) przyjść i szczegółowo objaśnić cały
  dokument — definicje, strukturę, wszystko. W przeciwnym razie **0 z części teoretycznej** i mocno
  obniżona ocena końcowa.
- **Termin:** poprawiona część teoretyczna miała być oddana razem z projektem końcowym — **4 czerwca**
  (uwaga: dziś jest 9 czerwca — termin minął). Wcześniejsze pytania/korekty przez Teams lub spotkanie.
- **Źródła LaTeX** wysłać razem z PDF-em.
- **Obecna ocena byłaby słaba** — prowadzący wstrzymuje ocenę do czasu poprawek; po porządnej
  rewizji ocena powinna znacząco wzrosnąć.
