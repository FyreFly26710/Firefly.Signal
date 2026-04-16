---
name: frontend-design
description: Create distinctive, production-grade frontend interfaces with a clear visual point of view. Use when design quality matters as much as functional correctness.
origin: ECC
---

# Frontend Design

Use this skill when the task is design-led, not just implementation-led.

## When To Use
- Building a new page, dashboard, or app surface.
- Upgrading a bland interface into something intentional.
- Translating a product idea into a concrete visual direction.
- Working on areas where typography, composition, color, and motion materially affect quality.

## Core Principle
Pick a direction and commit to it.

Safe, generic UI is usually worse than a coherent design with a clear personality.

## Design Workflow
### 1. Frame the surface
Settle:
- purpose
- audience
- tone
- visual direction
- one thing the user should remember

### 2. Build a small visual system
Define:
- typography hierarchy
- color variables
- spacing rhythm
- surface treatment
- motion rules

### 3. Compose with intention
- Use asymmetry, overlap, or dense layouts only when they improve hierarchy.
- Keep reading flow obvious even when the layout is bold.
- Avoid defaulting to interchangeable SaaS card grids.

### 4. Make motion meaningful
- Use animation to stage information and reinforce actions.
- Prefer one or two clear motion moments over scattered micro-interactions.

## Firefly-Specific Guardrails
- Preserve the established product language when editing an existing Firefly surface.
- Use MUI for accessible primitives and Tailwind for layout and token-driven styling.
- Keep desktop usage strong without breaking mobile comfort.
- Avoid default-library styling and obvious AI-generated visual patterns.

## Quality Gate
- The interface has a clear point of view.
- Typography and spacing feel intentional.
- Color and motion support the product instead of decorating it randomly.
- Accessibility and responsiveness still hold up.
