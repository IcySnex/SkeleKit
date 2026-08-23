<template>
  <section class="features-section">
    <div class="features-heading">
      <h2>What is SkeleKit?</h2>
    </div>

    <div class="feature-grid">
      <article class="feature">
        <h3>Native UIKit</h3>
        <p>Controls wrap UIKit types directly. SkeleKit owns composition and layout, never rendering.</p>
      </article>

      <article class="feature">
        <h3>Clean C# syntax</h3>
        <p>Define view trees with object initializers instead of XAML, view controllers or Auto Layout constraints.</p>
      </article>

      <article class="feature">
        <h3>Full AOT compatible</h3>
        <p>Bindings avoid reflection, expression trees, and runtime code generation.</p>
      </article>

      <article class="feature">
        <h3>Application services</h3>
        <p>Navigation, dependency injection, Dynamic Type, safe areas, keyboard avoidance, and accessibility are integrated.</p>
      </article>
    </div>
  </section>
</template>

<style scoped>
.features-section {
  width: min(1120px, calc(100% - 2rem));
  margin: 0 auto;
  padding: 4.5rem 0 3.5rem;
  border-top: 1px solid hsl(var(--border));
}

.features-heading {
  margin-bottom: 1.5rem;
}

.features-heading h2 {
  font-size: clamp(1.75rem, 3vw, 2.125rem);
  font-weight: 600;
  line-height: 1.2;
  letter-spacing: -0.025em;
}

.feature-grid {
  display: grid;
  overflow: hidden;
  grid-template-columns: repeat(4, minmax(0, 1fr));
  border: 1px solid hsl(var(--border));
  border-radius: calc(var(--radius) + 0.25rem);
  background: transparent;
}

.feature {
  padding: 1.75rem 1.5rem 1.25rem;
  background: transparent;
}

.feature + .feature {
  border-left: 1px solid hsl(var(--border));
}

.feature h3 {
  color: hsl(var(--brand));
  font-size: 1rem;
  font-weight: 600;
  letter-spacing: -0.015em;
}

.feature > p {
  margin-top: 0.75rem;
  color: hsl(var(--muted-foreground));
  font-size: 0.875rem;
  font-weight: 400;
  line-height: 1.65;
}

@media (max-width: 1000px) {
  .feature-grid {
    grid-template-columns: repeat(2, minmax(0, 1fr));
  }

  .feature + .feature {
    border-left: none;
  }

  .feature:nth-child(even) {
    border-left: 1px solid hsl(var(--border));
  }

  .feature:nth-child(n + 3) {
    border-top: 1px solid hsl(var(--border));
  }
}

@media (max-width: 640px) {
  .features-section {
    padding: 3rem 0 4rem;
  }

  .feature-grid {
    grid-template-columns: 1fr;
  }

  .feature:nth-child(even) {
    border-left: none;
  }

  .feature + .feature {
    border-top: 1px solid hsl(var(--border));
  }
}
</style>
