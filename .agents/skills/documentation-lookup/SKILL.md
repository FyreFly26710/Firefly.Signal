---
name: documentation-lookup
description: Use live library and framework documentation instead of stale memory when a task depends on current APIs, setup steps, or version-specific behavior.
origin: ECC
---

# Documentation Lookup

Use this skill when a task depends on accurate current behavior from a framework, library, SDK, or API.

## When To Use
- The user asks how a library or framework works.
- The answer depends on version-specific APIs or setup steps.
- You need a code example from official docs.
- The task mentions a concrete tool such as React, Vite, Tailwind, EF Core, MUI, Zustand, RabbitMQ, Docker, or Cloudflare.

## Working Rules
- Prefer live documentation tooling when it is available in the current harness.
- Otherwise browse official documentation or other primary sources.
- Do not answer from memory alone when the API surface may have changed.
- Prefer one authoritative source over stitching together conflicting summaries.
- Redact secrets before sending prompts or queries to any documentation tool.

## Lookup Flow
1. Identify the exact library, framework, or platform in question.
2. Resolve the official documentation source or the best live-docs identifier available.
3. Fetch the narrowest documentation relevant to the user's question.
4. Answer using the retrieved information, not guesses.
5. Call out version-specific behavior when it matters.

## Output Expectations
- Keep answers practical and implementation-oriented.
- Include a minimal example when the docs make one useful.
- State any uncertainty instead of inventing missing behavior.
