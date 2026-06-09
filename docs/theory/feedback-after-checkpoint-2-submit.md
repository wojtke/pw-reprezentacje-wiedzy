# Checkpoint Review – Knowledge Representation Systems

## Technical / Content Issues

1. **Restructure the document around two distinct languages:**
   - **Language of action description (AR-like):** Only simple actions exist here. Define syntax, semantics, and models — then close this section.
   - **Language of queries:** Compound/complex actions belong *here*, not in the action language. The `res` function over sets of actions, decompositions, and process semantics all go in the query language section.

2. **`impossible` and `initial` are not separate statement types** — they are abbreviations/instances of `causes` and `after`. Do not present them as independent constructs.

3. **The `res` function must be properly declared:** State its domain and codomain before using it. The same applies to `res_0`. Currently it appears out of nowhere.

4. **Multiple initial states → multiple models:** If `σ₀` is a *set* of initial states, you have multiple models. The semantics of `necessary` and `possible` must reflect this:
   - `necessary` = holds in every model and for every initial state
   - `possible` = holds in every model but there *exists* an initial state with a path
   - Interpreting `possible` as "there exists an initial state" is wrong — it must start from all models and all initial states.

5. **Move algorithmic/implementation content to the technical part of the report.** The theoretical part should define the language formally; how you implement it is a separate section.

## Writing / Structure Issues

6. **Section headings without content are unacceptable** — each section/subsection must have at least a brief description of what it covers.

7. **Add concrete examples throughout.** Formal definitions without examples are hard to follow; the cowboy/duel scenario was given as a good illustration. American CS textbooks were cited as a model of readable technical writing.

8. **Don't copy raw lecture notation without explanation.** If you use a formal expression, explain it in context.

## Administrative

9. **Absent team member (Sebastian)** must: (a) provide a formal excuse (medical certificate, dean's letter, etc.), and (b) come in and explain the entire document in detail — definitions, structure, everything. Failing that results in a 0 for the theoretical part and a dramatically lowered final grade.

10. **Deadline:** The corrected theoretical part must be submitted together with the final project — **June 4th**. Questions/corrections before then can be handled via Teams or a project meeting.

11. **LaTeX source** should be sent along with the PDF submission.

12. **Current grade would be poor** — the professor is withholding assessment pending corrections. After proper revision the grade should improve significantly.
