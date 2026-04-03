# Styling And Design System

This document defines the frontend styling direction for Firefly Signal.

## 1. Styling Stack

- MUI for accessible component primitives
- Tailwind CSS for layout and utility styling
- a small token layer for colors, spacing, radius, and typography

## 2. Design Direction

The product should feel deliberate and practical, not default-library generic.

Guidelines:
- clear visual hierarchy
- calm, high-contrast reading experience
- strong input and result affordances
- restrained use of accent color
- no dependency on default MUI look and feel

## 3. Token Strategy

Define a small token set early:
- color roles
- spacing scale
- radius scale
- shadow levels
- typography roles

Keep tokens stable and reuse them consistently.

## 4. MUI Usage Rules

Use MUI for:
- text fields
- buttons
- selects
- alerts
- dialogs
- menus
- accessible base surfaces where appropriate

Do not fight MUI component semantics just to force custom styling.

## 5. Tailwind Usage Rules

Use Tailwind for:
- page layout
- grids and flex layout
- spacing
- width and responsive behavior
- small visual refinements around MUI components

Avoid:
- arbitrary one-off values everywhere
- mixing contradictory utility patterns in the same component

## 6. Responsive Design

The app is web first, but should stay clean on mobile widths.

Rules:
- desktop gets the primary layout
- mobile gets a comfortable stacked layout
- forms should remain easy to use on small screens
- result cards should preserve scannability without horizontal overflow

## 7. Accessibility

Design and styling should preserve:
- visible focus states
- sufficient contrast
- readable text sizes
- accessible error and helper text
- keyboard-safe interactions

## 8. Reuse Rules

Create a shared design-system component only when:
- the same UI pattern repeats
- the abstraction reduces noise
- the abstraction still leaves feature intent clear

Do not build a large internal component library before the product has a real shape.
