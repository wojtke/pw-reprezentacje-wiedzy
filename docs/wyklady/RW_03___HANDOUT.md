# RW 03 – HANDOUT

> Source: `RW_03___HANDOUT.pdf`

---

## Knowledge Representation

    - **Lecture 3: Queries, Actions with Qualiﬁcations**
    - **dr Anna Maria Radzikowska**
    - Warsaw University of Technology
    - Faculty of Mathematics and Information Science
    - Building MiNI PW, room 504
    - E-mail: Anna.Radzikowska@pw.edu.pl
    - Warsaw 2026

---

## Outline

1. Recall: action language *AR*
2. Queries
  - Value queries
  - Executability queries
  - Accessibility queries
  - Goal queries
3. Satisﬁability
4. Actions with Qualiﬁcations
5. Action Language *AQ*
  - Semantics
6. Action Language *ARQ*
  - Syntax
  - Semantics

---

## Action Language *AR* : Syntax


### Statements

  - ***Value statement:***
    - *α* ***after*** *A* 1 *, . . . , A n* ***observable*** *α* ***after*** *A* 1 *, . . . , A n .*
  - ***Eﬀect statement:***
    - Action ***causes*** *α* ***if*** *π*
  - ***Releases statement:***
    - Action ***releases*** *f* ***if*** *π*
  - ***Constraint statement:***
    - ***always*** *α*
  - ***Fluent speciﬁcation statement:***
    - ***noninertial*** *f .*

---

## Action language *AR* : Semantics


### Structure

- A ***structure*** for a language *L* of the class *AR* is a triple *S* = (Σ *, σ* 0 *, Res* ) , where
  - Σ is a set of states
  - *σ* 0 *∈* Σ is the initial state
  - *Res* : *A c ×* Σ *→* 2 Σ is a transition function.

### Model

- A structure *S* = (Σ *, σ* 0 *, Res* ) is a ***model*** of an action domain *D* iﬀ
- **(M1)** Σ is the set of all states for *D* (all constraints are satisﬁed)
- **(M2)** every value statement and every observation statement is true in *S*
- **(M3)** for every *A ∈A c* and for every *σ ∈* Σ , *Res* ( *A, σ* ) is the set of all states *σ ′ ∈* Σ for
  - which the sets *New* ( *A, σ, σ ′* ) are minimal (wrt set inclusion), where *New* ( *A, σ, σ ′* ) is the set of literals *f* that hold in *σ ′* and
    - *f* is inertial and *σ* ( *f* ) *̸* = *σ ′* ( *f* ) , or
    - there is a statement A ***releases*** *f* ***if*** *π* in *D* such that *σ |* = *π* .

---

## Queries

![Queries](images/RW_03___HANDOUT_p004_diagram.png)


---

## Queries

- Value queries
  - ***necessary*** *α* ***after*** *A* 1 *, . . . , A n* ***from*** *π* ***possibly*** *α* ***after*** *A* 1 *, . . . , A n* ***from*** *π*
- The 1st query states that *α* **always** holds after performing the sequence *A* 1 *, . . . , A n* of actions from any state satisfying *π* . The 2nd query says that *α* **sometimes** holds after executing *A* 1 *, . . . , A n* from any state satisfying *π* . When the option ***from*** *π* is omitted, these queries refer to the initial state.

---

## Queries (cont.)


### Executability queries

  - ***necessary executable*** *A* 1 *, . . . , A n* ***from*** *π* ***possibly executable*** *A* 1 *, . . . , A n* ***from*** *π .*
- Intuitively, the 1 *st* (resp. the 2 *nd* ) query states that from every state satisfying *π* the sequence ( *A* 1 *, . . . , A n* ) of actions is **always** (resp. **sometimes** ) executable.
- Again, if the phrase ***from*** *π* is omitted, then the initial state is to be taken into account.

---

## Queries (cont.)

- Accessibility queries
  - ***necessary accessible*** *γ* ***from*** *π* ***possibly accessible*** *γ* ***from*** *π .*
- These queries state that the goal *γ* is **always** (resp. **sometimes** ) achieved from any state satisfying *π* .
- Intuitively:
  - *There exists a sequence* ( *A* 1 *, . . . , A n* ) *, n ­* 0 *, of actions, execu-* *table from any state satisfying π which always (resp. ever) leads* *to states satisfying γ . Diﬀerent sequences of actions, starting in* *diﬀerent states where π holds, may lead to states satisfying the* *goal γ .*
- As before, if ***from*** *π* is omitted, then it refers to the initial state.

---

## Queries (cont.)


### Goal query

  - ***goal*** *γ* ***from*** *π*
- This query returns the shortes sequence ( *A* 1 *, . . . , A n* ) of actions, executable from any state satisfying *π* (if such a sequence exists!), which leads to states satisfying the goal condition *γ* .
- If the phrase “ ***from*** *π* ” is omitted, then the goal *γ* is to be achieved from the initial state.

---

## Satisﬁability

- Let *D* be an action domain and let *Q* be a query of the language *AR* . A query is a ***consequence of*** *D* , in symbols *D |≈ Q* , iﬀ
- ***necessary*** *α* ***after*** *A* 1 *, . . . , A n* ***from*** *π*
- *D |≈ Q* iﬀfor every model *S* = (Σ *, σ* 0 *, Res* ) of *D* , for every state *σ ∈* Σ and for **every** mapping Ψ *s* , *σ |* = *π* implies Ψ *S* (( *A* 1 *, . . . , A n* ) *, σ* ) *|* = *α* .
- ***necessary*** *α* ***after*** *A* 1 *, . . . , a n*
- *D |≈ Q* iﬀfor every model *s* = (Σ *, σ* 0 *, Res* ) of *D* , and for **every** mapping Ψ *S* , it holds Ψ *S* (( *a* 1 *, . . . , A n* ) *, σ* 0 ) *|* = *α* .

---

## Satisﬁability (cont.)

- *Q* : ***possibly*** *α* ***after*** *A* 1 *, . . . , A n* ***from*** *π*
- *D |≈ Q* iﬀfor every model *S* = (Σ *, σ* 0 *, Res* ) of *D* , for every state *σ ∈* Σ , and for **some** mapping Ψ *s* , if *σ |* = *π* , then Ψ *S* (( *A* 1 *, . . . , A n* ) *, σ* ) *|* = *α* .
- *Q* : ***possibly*** *α* ***after*** *A* 1 *, . . . , A n*
- *D |≈ Q* iﬀfor every model *S* = (Σ *, σ* 0 *, Res* ) of *D* and for **some** mapping Ψ *s* , it holds Ψ *S* (( *A* 1 *, . . . , A n* ) *, σ* 0 ) *|* = *α* .

---

## Example: Tossing a coin


### Example 3.1

- Let an action domain be as follows:
    - ***initially*** *heads* ; Toss ***releases*** *heads* .
- Then
    - *D |≈* ***possibly*** *heads* ***after*** Toss *D |≈* ***possibly*** *¬ heads* ***after*** Toss *D |̸≈* ***necessary*** *¬ heads* ***after*** Toss *D |̸≈* ***necessary*** *head* ***after*** Toss .

---

## Satisﬁability (cont.)

- *Q* : ***necessary executable*** *A* 1 *, . . . , A n* ***from*** *π*
- *D |≈ Q* iﬀfor every model *S* = (Σ *, σ* 0 *, Res* ) of *D* , for every state *σ ∈* Σ , and for **every** mapping Ψ *S* , if *σ |* = *π* , then Ψ *S* (( *A* 1 *, . . . , A n* ) *, σ* ) is deﬁned.
- *Q* : ***necessary executable*** *A* 1 *, . . . , A n*
- *D |≈ Q* iﬀfor every model *S* = (Σ *, σ* 0 *, Res* ) of *D* and for **every** mapping Ψ *S* , Ψ *S* (( *A* 1 *, . . . , A n* ) *, σ* 0 ) is deﬁned.

---

## Satisﬁability (cont.)

- *Q* : ***possibly executable*** *A* 1 *, . . . , A n* ***from*** *π*
- *D |≈ Q* iﬀfor every model *S* = (Σ *, σ* 0 *, Res* ) of *D* , for every state *σ ∈* Σ , and for **some** mapping Ψ *S* , if *σ |* = *π* , then Ψ *S* (( *A* 1 *, . . . , A n* ) *, σ* ) is deﬁned.
- *Q* : ***possibly executable*** *A* 1 *, . . . , A n*
- *D |≈ Q* iﬀfor every model *S* = (Σ *, σ* 0 *, Res* ) of *D* and for **some** mapping Ψ *S* , Ψ *S* (( *A* 1 *, . . . , A n* ) *, σ* 0 ) is deﬁned.

---

## Satisﬁability (cont.)


### *Q* : ***necessary accessible*** *γ* ***from*** *π*

- *D |≈ Q* iﬀfor every model *S* = (Σ *, σ* 0 *, Res* ) of *D* , for every state *σ ∈* Σ , and for **every** mappings Ψ *S* , if *σ |* = *π* , then there is some sequence ( *A* 1 *, . . . , A k* ) , *k* ⩾ 0 , of actions such that
  - Ψ *S* (( *A* 1 *, . . . , A k* ) *, σ* ) is deﬁned;
  - Ψ *S* (( *A* 1 *, . . . , A k* ) *, σ* ) *|* = *γ* .

### *Q* : ***necessary accessible*** *γ*

- *D |≈ Q* iﬀfor every model *S* = (Σ *, σ* 0 *, Res* ) of *D* and for **every** mappings Ψ *S* , there is some sequence ( *A* 1 *, . . . , A k* ) , *k* ⩾ 0 , of actions such that
  - Ψ *S* (( *A* 1 *, . . . , A k* ) *, σ* 0 ) is deﬁned;
  - Ψ *S* (( *A* 1 *, . . . , A k* ) *, σ* 0 ) *|* = *γ* .

---

## Satisﬁability (cont.)


### *Q* : ***possibly accessible*** *γ* ***from*** *π*

- *D |≈ Q* iﬀfor every model *S* = (Σ *, σ* 0 *, Res* ) of *D* , for every state *σ ∈* Σ , and for **some** mappings Ψ *S* , if *σ |* = *π* , then there is some sequence ( *A* 1 *, . . . , A k* ) , *k* ⩾ 0 , of actions such that
  - Ψ *S* (( *A* 1 *, . . . , A k* ) *, σ* ) is deﬁned;
  - Ψ *S* (( *A* 1 *, . . . , A k* ) *, σ* ) *|* = *γ* .

### *Q* : ***possibly accessible*** *γ*

- *D |≈ Q* iﬀfor every model *S* = (Σ *, σ* 0 *, Res* ) of *D* and for **some** mappings Ψ *S* , there is some sequence ( *A* 1 *, . . . , A k* ) , *k* ⩾ 0 , of actions such that
  - Ψ *S* (( *A* 1 *, . . . , A k* ) *, σ* 0 ) is deﬁned;
  - Ψ *S* (( *A* 1 *, . . . , A k* ) *, σ* 0 ) *|* = *γ* .

---

## Example


### Example 3.2

- Consider an action domain:
    - ActionA ***causes*** *p ∨ q* ; ActionB ***causes*** *¬ r* ; ***impossible*** ActionB ***if*** *¬ q* .
    - *p* ,  *q* , *r*
    - A
    -  *p* ,  *q* , *r*
    - A
    -  *p* , *q* , *r*
    -  *p* , *q* ,  *r*
    - B
    - A
    - A , B

---

## Example (cont.)


### Example 3.2 (cont.)

    - *p* ,  *q* , *r*
    - A
    -  *p* ,  *q* , *r*
    - A
    -  *p* , *q* , *r*
    -  *p* , *q* ,  *r*
    - B
    - A
    - A , B
- Note:
  - *D |̸≈* ***necessary accessible*** *¬ r* ***from*** *¬ p .* *D |≈* ***possibly accessible*** *¬ r* ***from*** *¬ p .*

---

## Satisﬁability (cont.)


### *Q* : ***goal*** *γ* ***from*** *π*

- *D |≈ Q* iﬀthere is some sequence ( *A* 1 *, . . . , A n* ) of action such that for every models *S* = (Σ *, σ* 0 *, Res* ) of *D* , for every state *σ ∈* Σ , and for **every** mapping Ψ *S* , if *σ |* = *π* , then
  - Ψ *S* (( *A* 1 *, . . . , A n* ) *, σ* ) is deﬁned;
  - Ψ *S* (( *A* 1 *, . . . , A n* ) *, σ* ) *|* = *γ* .
- The sequence ( *A* 1 *, . . . , A n* ) , if exists, is called an ***answer for*** the query *Q* wrt *D* .

### *Q* : ***goal*** *γ*

- *D |≈ Q* iﬀthere is some sequence ( *A* 1 *, . . . , A n* ) of action such that for every models *S* = (Σ *, σ* 0 *, Res* ) of *D* and for **every** mapping Ψ *S* ,
  - Ψ *S* (( *A* 1 *, . . . , A n* ) *, σ* 0 ) is deﬁned;
  - Ψ *S* (( *A* 1 *, . . . , A n* ) *, σ* 0 ) *|* = *γ* .

---

## Actions with Qualiﬁcations

![Actions with Qualiﬁcations](images/RW_03___HANDOUT_p019_diagram.png)


---

## Qualiﬁcation problem

  - *The qualiﬁcation problem concerns conditions under which the* *performance of an action is possible, it proceeds successfully and* *leads to expected results. Since all preconditions of actions are* *usually immense, it is usually unreasonable (if ever possible) to* *explicitly enumerate and check all of possibilities (even most unli-* *kely).*

---

## Introductory example (Lin & Reiter, 1994)


### Example 3.3

- Consider the following scenario:
  - *In an ancient kingdom there are two blocks, A and B. Either block* *may be painted yellow, but by order of the emperor at most one* *of the blocks is permitted to be yellow at a time. Initially the ﬁrst* *block is yellow. A painter tried to paint the second one yellow.*
- Representation in *AR* :
    - ***always*** *¬ yellowA ∨¬ yellowB* ; ***initially*** *yellowA* ; PaintA ***causes*** *yellowA* ; PaintB ***causes*** *yellowB* .

---

## Introductory example (cont.)


### Example 3.3 (cont.)

- Here Σ = *{ σ* 0 *, σ* 1 *, σ* 2 *}* where
  - *σ* 0 = *{ yellowA, ¬ yellowB }*
    - *σ* 1 = *{¬ yellowA, ¬ yellowB } .*
  - *σ* 2 = *{¬ yellowA, yellowB }*
- Note that
    - *Res* 0 ( PaintB *, σ* 0 ) = *Res* ( PaintB *, σ* 0 ) = *{ σ* 2 *}* .
- **So painting the block B changes the color of the block A!**

---

## Example: Enticing Fred


### Example 3.3

- Consider the following action domain:
    - ***initially*** *loaded ∧ walking* ; ***always*** *walking → alive* ; Shoot ***causes*** *¬ loaded* ; Shoot ***causes*** *¬ alive* ***if*** *loaded* ; Entice ***causes*** *walking* .
- Consider the following states
    - *σ* 0 = *{ alive, loaded, walking }*
    - *σ* 2 = *{¬ alive, ¬ loaded, ¬ walking } .*
- Note that
    - *Res* ( Shoot *, σ* 0 ) = *{ σ* 1 *}* .

---

## Example: Enticing Fred (cont.)


### Example 3.3 (cont.)

- Furthermore,
    - *σ* 2 = *{¬ alive, loaded, ¬ walking }* *σ* 3 = *{ alive, ¬ loaded, walking }* *σ* 4 = *{ alive, loaded, walking }* .
- In *AR* we get:
    - *Res* 0 ( Entice *, σ* 2 ) = *{ σ* 3 *, σ* 4 *}* *New* ( Entice *, σ* 2 *, σ* 3 ) = *{ alive, ¬ loaded, walking }* * *New* ( Entice *, σ* 2 *, σ* 4 ) = *{ alive, walking }*
    - *Res* ( Entice *, σ* 2 ) = *{ σ* 4 *}* .
- **So enticing a dead turkey makes him miraculously alive!!!**

### The reasoning method of *AR* is **NOT** adequate to represent this scenario. Another language is needed!


---

## Basic assumptions

  - Inertia law.
  - Complete information about all actions and all ﬂuents.
  - Nondeterminism is allowed.
  - **Whenever any indirect precondition of an action fails,** **the action is unexecutable** .
  - Action can be unexecutable is some states.

### Action language *AQ*

- To represent this class of dynamic systems we will use action languages of the class *AQ* .

---

## Action Language *AQ*


### Syntax

- Identical as in *AR* .

### Semantics

- Let *D* be an action domain in a language *L* of the class *AQ* . A ***structure*** for a language *L* is a triple *S* = (Σ *, σ* 0 *, Res* ) such that
  - Σ is a set of states;
  - *σ* 0 *∈* Σ is the initial state;
  - *Res* : *A c ×* Σ *→* 2 Σ is a transition function.

---

## Semantics of *AQ* (cont.)


### Notation

  - *D −* : the action domain obtained from *D* by removing all constraint statements.
  - *Mod* ( *D −* ) : the set of all models *S −* = (Σ *− , σ* 0 *, Res −* ) of *D −*
  - (in the sense of *AR* ) where *σ* 0 satisﬁes all constraints in *D* .

### Deﬁnition 3.1

- Le *D* be an action domain and let (Σ *− , σ* 0 *, Res −* ) *∈ Mod* ( *D −* ) . A ***model*** of *D* is a structure (Σ *, σ* 0 *, Res* ) such that
  - Σ *⊆* Σ *−* is the set of all states satisfying all constraints in *D* ;
  - for every *A ∈A c* and for every *σ ∈* Σ , *Res* ( *A, σ* ) = *Res −* ( *A, σ* ) *∩* Σ .

---

## Introductory example (cont.)


### Example 3.3 (cont.)

    - ***always*** *¬ yellowA ∨¬ yellowB* ; ***initially*** *yellowA* ; PaintA ***causes*** *yellowA* ; PaintB ***causes*** *yellowB* .
- Now Σ *−* = *{ σ* 0 *, σ* 1 *, σ* 2 *, σ* 3 *}* where
  - *σ* 0 = *{ yellowA, ¬ yellowB }*
    - *σ* 2 = *{¬ yellowA, yellowB }*
  - *σ* 1 = *{¬ yellowA, ¬ yellowB }*
    - *σ* 3 = *{ yellowA, yellowB }* .
    - *Res* 0 ( PaintB *, σ* 0 ) = *{ σ* 2 *, σ* 3 *}* *New* ( PaintB *, σ* 0 *, σ* 2 ) = *{¬ yellowA, yellowB }* * *New* ( PaintB *, σ* 0 *, σ* 3 ) = *{ yellowB }*
    - *Res −* ( PaintB *, σ* 0 ) = *{ σ* 3 *}* *Res* ( PaintB *, σ* 0 ) = *∅* .

---

## Introductory example (cont.)


### Example 3.3 (cont.)

  - *σ* 0 = *{ yellowA, ¬ yellowB }*
    - *σ* 2 = *{¬ yellowA, yellowB }*
  - *σ* 1 = *{¬ yellowA, ¬ yellowB }*
    - *σ* 3 = *{ yellowA, yellowB }* .
- Analogously:
    - *Res −* 0 ( PaintA *, σ* 2 ) = *{ σ* 0 *, σ* 3 *}* *New* ( PaintA *, σ* 2 *, σ* 0 ) = *{ yellowA, ¬ yellowB }* * *New* ( PaintA *, σ* 2 *, σ* 3 ) = *{ yellowA }*
    - *Res −* ( PaintA *, σ* 2 ) = *{ σ* 3 *}* *Res* ( PaintA *, σ* 2 ) = *∅* .

---

## Introductory example (cont.)


### Example 3.3 (cont.)

    - PaintA
    -  *yellowA,*  *yellowB*
    - *yellowA,*  *yellowB*
    - PaintA
    - PaintB PaintB
    - PaintA
    -  *yellowA,* *yellowB*
    - *yellowA,* *yellowB*
    - PaintA , PaintB
    - PaintB
    - Fig.3.1 : Model of D **–**

### Example 3.3 (cont.)


---

## Example: Enticing Fred (cont.)


### Example 3.3 (cont.)

    - ***initially*** *loaded ∧ walking* ; ***always*** *walking → alive* ; Shoot ***causes*** *¬ loaded* ; Shoot ***causes*** *¬ alive* ***if*** *loaded* ; Entice ***causes*** *walking* .
- Σ *−* = *{ σ* 0 *, . . . , σ* 7 *}* where
- *σ* 0 = *{ alive, loaded, walking }*
    - *σ* 4 = *{ alive, loaded, ¬ walking }*
- *σ* 1 = *{ alive, ¬ loaded, walking }*
    - *σ* 5 = *{ alive, ¬ loaded, ¬ walking }*
- *σ* 2 = *{¬ alive, loaded, walking }*
    - *σ* 6 = *{¬ alive, loaded, ¬ walking }*
- *σ* 3 = *{¬ alive, ¬ loaded, walking }*
    - *σ* 7 = *{¬ alive, ¬ loaded, ¬ walking }* .

---

## Example: Enticing Fred (cont.)


### Example 3.3 (cont.)

- *σ* 0 = *{ alive, loaded, walking }*
    - *σ* 4 = *{ alive, loaded, ¬ walking }*
- *σ* 1 = *{ alive, ¬ loaded, walking }*
    - *σ* 5 = *{ alive, ¬ loaded, ¬ walking }*
- *σ* 2 = *{¬ alive, loaded, walking }*
    - *σ* 6 = *{¬ alive, loaded, ¬ walking }*
- *σ* 3 = *{¬ alive, ¬ loaded, walking }*
    - *σ* 7 = *{¬ alive, ¬ loaded, ¬ walking }* .
    - *Res −* 0 ( Entice *, σ* 6 ) = *{ σ* 0 *, σ* 1 *, σ* 2 *, σ* 3 *}* *New* ( Entice *, σ* 6 *, σ* 0 ) = *{ alive, walking }* *New* ( Entice *, σ* 6 *, σ* 1 ) = *{ alive, ¬ loaded, walking }* * *New* ( Entice *, σ* 6 *, σ* 2 ) = *{ walking }* *New* ( Entice *, σ* 6 *, σ* 3 ) = *{¬ loaded, walking }*
    - *Res −* ( Entice *, σ* 6 ) = *{ σ* 2 *}* *Res* ( Entice *, σ* 6 ) = *∅* .

---

## Example: Enticing Fred (cont.)


### Example 3.3 (cont.)

- *σ* 0 = *{ alive, loaded, walking }*
    - *σ* 4 = *{ alive, loaded, ¬ walking }*
- *σ* 1 = *{ alive, ¬ loaded, walking }*
    - *σ* 5 = *{ alive, ¬ loaded, ¬ walking }*
- *σ* 2 = *{¬ alive, loaded, walking }*
    - *σ* 6 = *{¬ alive, loaded, ¬ walking }*
- *σ* 3 = *{¬ alive, ¬ loaded, walking }*
    - *σ* 7 = *{¬ alive, ¬ loaded, ¬ walking }* .
- Consider the shooting action in the initial state *σ* 0 .
    - *Res −* 0 ( Shoot *, σ* 0 ) = *{ σ* 3 *, σ* 7 *}* * *New* ( Shoot *, σ* 0 *, σ* 3) = *{¬ alive, ¬ loaded }* *New* ( Shoot *, σ* 0 *, σ* 7 ) = *{¬ alive, ¬ loaded, ¬ walking }*
    - *Res −* ( Shoot *, σ* 0 ) = *{ σ* 3 *}* *Res* ( Shoot *, σ* 0 ) = *∅* .
- **So it is impossible to shoot a walking!**

---

## Problem


### Question

- *Should we reject inadmissible states* ***before*** *or* ***after*** *minimization of* *changes?*

### Answer

  - If constraints inﬂuence only on actions’ **ramiﬁcations** and do not inﬂuence on actions’ qualiﬁcations, then inadmissible states are to be rejected **before** minimization of changes
  - If constraints inﬂuence only on actions’ **qualiﬁcations** and do not inﬂuence on actions’ ramﬁcations, then inadmissible states should be rejected **after** minimization of changes.

---

## Another problem


### Question

  - *What should be done if a constraint works for ramiﬁcations wrt* *one action, and at the same time for qualiﬁcations wrt another* *one?*

### Possible solutions

- Mixed reasoning methods: action languages *ARQ* and *AQR* .

---

## Action Language *ARQ*


### Syntax

- The set of statements of *AR* is extended by an ﬂuent preserving statement:
    - A ***preserves*** *f* ***if*** *π*
- Intuitively, in any state satisfying *π* , the action A , when performed, **does** **not** change the value of the ﬂuent *f*

---

## Action Language *ARQ* – semantics


### Structure

- A ***structure*** for a language *L* of the class *ARQ* is a triple *S* = (Σ *, σ* 0 *, Res* ) where
  - Σ is a set of states;
  - *σ* 0 *∈* Σ is the initial state;
  - *Res* : *A c ×* Σ *→* 2 Σ is a transition function.

### Model

- Let *D* be an action domain in *ARQ* and let *D −* stand for the action domain obtained from *D* by removing all ﬂuent preserving statements. Let *Mod* ( *D −* ) be the set of all models od *D −* in the sense of *AR* . A structure *S* = (Σ *, σ* 0 *, Res* ) is a ***model*** of *D* iﬀfor every action *A ∈A c* and for every state *σ ∈* Σ ,
- *Res* ( *A, σ* )= *{ σ ′ ∈* Σ : ( *A* ***preserves*** *f* if *π* ) *∈ D ∧* ( *σ |* = *π* ) *⇒ σ* ( *f* )= *σ ′* ( *f* ) *} .*

---

## Example: Enticing Fred (cont.)


### Example 3.3 (cont.)

    - ***initially*** *loaded ∧ walking* ; ***always*** *walking → alive* ; Shoot ***causes*** *¬ loaded* ; Shoot ***causes*** *¬ alive* ***if*** *loaded* ; Entice ***causes*** *walking* ; Entice ***preserves*** *alive* ***if*** *loaded* .
- Here Σ = *{ σ* 0 *, σ* 1 *, . . . , σ* 5 *}* where
- *σ* 0 = *{ alive, loaded, walking }*
    - *σ* 3 = *{ alive, ¬ loaded, walking }*
- *σ* 1 = *{ alive, loaded, ¬ walking }*
    - *σ* 4 = *{ alive, ¬ loaded, ¬ walking }*
- *σ* 2 = *{¬ alive, loaded, ¬ walking }*
    - *σ* 3 = *{¬ alive, ¬ loaded, ¬ walking }*

---

## Example: Enticing Fred (cont.)


### Example 3.3 (cont.)

- *σ* 0 = *{ alive, loaded, walking }*
    - *σ* 3 = *{ alive, ¬ loaded, walking }*
- *σ* 1 = *{ alive, loaded, ¬ walking }*
    - *σ* 4 = *{ alive, ¬ loaded, ¬ walking }*
- *σ* 2 = *{¬ alive, loaded, ¬ walking }*
    - *σ* 3 = *{¬ alive, ¬ loaded, ¬ walking }*
    - *Res −* 0 ( Entice *, σ* 2 ) = *{ σ* 0 *, σ* 3 *}* * *New* ( Entice *, σ* 2 *, σ* 0 ) = *{ alive, walking }* *New* ( Entice *, σ* 2 *, σ* 3 ) = *{ alive, ¬ loaded, walking }* *Res −* ( Entice *, σ* 2 ) = *{ σ* 0 *}*
- Since *σ* 2 *|* = *¬ alive ∧ loaded* and *σ* 0 *|* = *alive* , we get *σ* 0 *̸∈ Res* ( Entice *, σ* 2 ) . Hence
    - *Res* ( Entice *, σ* 2 ) = *∅ .*

---

## Example: Enticing Fred (cont.)


### Example 3.3 (cont.)

- *σ* 0 = *{ alive, loaded, walking }*
    - *σ* 3 = *{ alive, ¬ loaded, walking }*
- *σ* 1 = *{ alive, loaded, ¬ walking }*
    - *σ* 4 = *{ alive, ¬ loaded, ¬ walking }*
- *σ* 2 = *{¬ alive, loaded, ¬ walking }*
    - *σ* 3 = *{¬ alive, ¬ loaded, ¬ walking }*
    - *Res −* 0 ( Shoot *, σ* 0 ) = *Res −* 0( Shoot *, σ* 0 ) = *{ σ* 5 *}* .
- Since there is no ﬂuent preserving statement wrt Shoot and *walking* , we obtain
    - *Res* ( Shoot *, σ* 0 ) = *{ σ* 5 *} .*

---

## Alternative solution


### Representation in *AQ*

- We can use an *AQ* language: whenever constraints work for actions’ ramiﬁcations, the respective ***releases*** statements are to be added.

### Example 3.3 (cont.)

    - ***initially*** *loaded ∧ walking* ; ***always*** *walking → alive* ; Shoot ***causes*** *¬ loaded* ; Shoot ***causes*** *¬ alive* ***if*** *loaded* ; Shoot ***releases*** *walking* ***if*** *loaded* ; Entice ***causes*** *walking* .

---

## Example: Enticing Fred (cont.)


### Example 3.3 (cont.)

- *σ* 0 = *{ alive, loaded, walking }*
    - *σ* 4 = *{ alive, loaded, ¬ walking }*
- *σ* 1 = *{ alive, ¬ loaded, walking }*
    - *σ* 5 = *{ alive, ¬ loaded, ¬ walking }*
- *σ* 2 = *{¬ alive, loaded, walking }*
    - *σ* 6 = *{¬ alive, loaded, ¬ walking }*
- *σ* 3 = *{¬ alive, ¬ loaded, walking }*
    - *σ* 7 = *{¬ alive, ¬ loaded, ¬ walking }* .
    - *Res −* 0 ( Shoot *, σ* 0 ) = *{ σ* 3 *, σ* 7 *}* * *New* ( Shoot *, σ* 0 *, σ* 3 ) = *{¬ alive, ¬ loaded, walking }* * *New* ( Shoot *, σ* 0 *, σ* 7 ) = *{¬ alive, ¬ loaded, ¬ walkikng }* *Res −* ( Shoot *, σ* 0 ) = *{ σ* 3 *, σ* 7 *}*
    - *Res* ( Shoot *, σ* 0 ) = *{ σ* 7 *}* .

---

## Thank you for your attention!

![Thank you for your attention!](images/RW_03___HANDOUT_p043_diagram.png)


---

