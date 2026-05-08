"""Główne okno (O7) — three-zone layout per DESIGN §3."""
from __future__ import annotations
import tkinter as tk
from tkinter import ttk


def build_window(root: tk.Tk) -> None:
    root.title("DS4 Reasoner — procesy działań złożonych")
    root.geometry("1100x750")

    menubar = tk.Menu(root)
    for label in ("Plik", "Przykłady", "Pomoc"):
        m = tk.Menu(menubar, tearoff=False)
        menubar.add_cascade(label=label, menu=m)
    root.config(menu=menubar)

    paned = ttk.PanedWindow(root, orient="horizontal")
    paned.pack(fill="both", expand=True, padx=8, pady=8)

    domain_frame = ttk.LabelFrame(paned, text="Dziedzina")
    paned.add(domain_frame, weight=1)
    tk.Text(domain_frame, wrap="none").pack(fill="both", expand=True, padx=4, pady=4)

    right = ttk.PanedWindow(paned, orient="vertical")
    paned.add(right, weight=1)

    query_frame = ttk.LabelFrame(right, text="Kwerenda")
    right.add(query_frame, weight=1)
    tk.Text(query_frame, height=4, wrap="word").pack(fill="x", padx=4, pady=4)
    ttk.Button(query_frame, text="Oblicz").pack(anchor="e", padx=4, pady=4)

    result_frame = ttk.LabelFrame(right, text="Wynik")
    right.add(result_frame, weight=2)
    ttk.Label(result_frame, text="—", anchor="nw").pack(fill="both", expand=True, padx=4, pady=4)

    status = ttk.Label(root, text="● Gotowe", anchor="w", relief="sunken")
    status.pack(fill="x", side="bottom")


def run() -> None:
    root = tk.Tk()
    build_window(root)
    root.mainloop()
