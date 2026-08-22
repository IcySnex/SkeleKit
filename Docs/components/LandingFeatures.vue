<template>
  <section class="features-section">
    <div class="features-heading">
      <p class="eyebrow">Framework overview</p>
      <h2>What is SkeleKit?</h2>
      <p>SkeleKit provides C# APIs for UIKit view composition, layout, bindings, navigation, and application setup.</p>
    </div>

    <div class="feature-grid">
      <article class="feature feature-native">
        <div class="feature-icon"><SmartIcon name="lucide:app-window-mac" :size="22" /></div>
        <div>
          <h3>Native UIKit controls</h3>
          <p>Controls wrap UIKit types directly. SkeleKit owns composition and layout, never rendering.</p>
        </div>
      </article>

      <article class="feature feature-code">
        <div class="feature-icon"><SmartIcon name="lucide:braces" :size="22" /></div>
        <div>
          <h3>Clean C# syntax</h3>
          <p>Define view trees with object initializers instead of XAML, view controllers or Auto Layout constraints.</p>
        </div>
      </article>

      <article class="feature feature-aot">
        <div class="feature-icon"><SmartIcon name="lucide:shield-check" :size="22" /></div>
        <div>
          <h3>Full AOT compatible</h3>
          <p>Bindings avoid reflection, expression trees, and runtime code generation.</p>
        </div>
      </article>

      <article class="feature feature-layout">
        <div class="feature-icon"><SmartIcon name="lucide:layout-grid" :size="22" /></div>
        <div>
          <h3>Layout containers</h3>
          <p>Grid, StackPanel, Overlay, Border, and ScrollView use a testable two-pass measure and arrange engine.</p>
        </div>
      </article>

      <article class="feature feature-system">
        <div class="feature-icon"><SmartIcon name="lucide:boxes" :size="22" /></div>
        <div>
          <h3>Application services</h3>
          <p>Navigation, dependency injection, Dynamic Type, safe areas, keyboard avoidance, and accessibility are integrated.</p>
        </div>
      </article>
    </div>
  </section>
</template>

<style scoped>
.features-section {
  width: min(1180px, calc(100% - 2rem));
  margin: 0 auto;
  padding: 2rem 0 7rem;
}

.features-heading {
  max-width: 680px;
  margin-bottom: 2.5rem;
}

.features-heading .eyebrow {
  margin-bottom: 0.75rem;
  color: hsl(var(--brand));
  font-size: 0.75rem;
  font-weight: 600;
  letter-spacing: 0.12em;
  text-transform: uppercase;
}

.features-heading h2 {
  font-size: clamp(1.875rem, 3vw, 2.25rem);
  font-weight: 600;
  line-height: 1.15;
  letter-spacing: -0.025em;
}

.features-heading > p:last-child {
  margin-top: 1rem;
  color: hsl(var(--muted-foreground));
  font-size: 0.9375rem;
  font-weight: 400;
  line-height: 1.6;
}

.feature-grid {
  display: grid;
  grid-template-columns: repeat(12, minmax(0, 1fr));
  gap: 1rem;
}

.feature {
  --feature-tint-x: 92%;
  --feature-tint-y: 8%;
  position: relative;
  display: flex;
  min-height: 210px;
  flex-direction: column;
  justify-content: space-between;
  overflow: hidden;
  border: 1px solid hsl(var(--border));
  border-radius: calc(var(--radius) + 0.25rem);
  background:
    radial-gradient(
      circle at var(--feature-tint-x) var(--feature-tint-y),
      hsl(var(--brand) / 0.12),
      transparent 46%
    ),
    linear-gradient(145deg, hsl(var(--card)), hsl(var(--brand-secondary) / 0.025));
  padding: 1.5rem;
}

.feature::after {
  content: '';
  position: absolute;
  width: 190px;
  height: 190px;
  top: -100px;
  right: -90px;
  border: 1px solid hsl(var(--brand) / 0.18);
  border-radius: 999px;
  box-shadow: 0 0 0 28px hsl(var(--brand) / 0.055), 0 0 0 56px hsl(var(--brand) / 0.025);
  pointer-events: none;
}

.feature-icon {
  display: grid;
  width: 2.75rem;
  height: 2.75rem;
  place-items: center;
  border: 1px solid hsl(var(--brand) / 0.22);
  border-radius: var(--radius);
  background: linear-gradient(145deg, hsl(var(--brand) / 0.15), hsl(var(--brand-secondary) / 0.06));
  color: hsl(var(--brand));
  box-shadow: 0 1px 0 hsl(var(--foreground) / 0.04);
}

.feature h3 {
  margin-top: 2.5rem;
  font-size: 1rem;
  font-weight: 600;
  letter-spacing: -0.015em;
}

.feature p {
  max-width: 46rem;
  margin-top: 0.5rem;
  color: hsl(var(--muted-foreground));
  font-size: 0.875rem;
  font-weight: 400;
  line-height: 1.6;
}

.feature-native {
  grid-column: span 7;
  min-height: 250px;
}

.feature-native .feature-icon {
  border-radius: 32%;
}

.feature-code {
  --feature-tint-x: 12%;
  --feature-tint-y: 92%;
  grid-column: span 5;
}

.feature-code::after {
  border-radius: var(--radius);
  transform: rotate(18deg);
}

.feature-code .feature-icon {
  border-radius: 999px;
}

.feature-aot {
  --feature-tint-x: 88%;
  --feature-tint-y: 88%;
  grid-column: span 4;
}

.feature-aot::after {
  width: 150px;
  height: 150px;
  top: -88px;
  right: -62px;
  border-radius: calc(var(--radius) + 0.25rem);
  box-shadow: 0 0 0 22px hsl(var(--brand) / 0.05), 0 0 0 44px hsl(var(--brand) / 0.025);
  transform: rotate(45deg);
}

.feature-aot .feature-icon {
  border-radius: 35% 35% 50% 50%;
}

.feature-layout {
  --feature-tint-x: 10%;
  --feature-tint-y: 10%;
  grid-column: span 5;
  min-height: 240px;
}

.feature-layout::after {
  border-radius: calc(var(--radius) + 0.25rem);
  background-image:
    linear-gradient(to right, hsl(var(--brand) / 0.12) 1px, transparent 1px),
    linear-gradient(to bottom, hsl(var(--brand) / 0.12) 1px, transparent 1px);
  background-size: 24px 24px;
  transform: rotate(10deg);
}

.feature-layout .feature-icon {
  border-radius: calc(var(--radius) - 0.2rem);
}

.feature-system {
  --feature-tint-x: 85%;
  --feature-tint-y: 20%;
  grid-column: span 3;
}

.feature-system::after {
  border-style: dashed;
  border-radius: 999px 999px var(--radius) 999px;
  box-shadow: 0 0 0 28px hsl(var(--brand) / 0.045), 0 0 0 56px hsl(var(--brand) / 0.02);
}

.feature-system .feature-icon {
  width: 3.4rem;
  border-radius: 999px;
}

@media (max-width: 900px) {
  .feature-native,
  .feature-code,
  .feature-aot,
  .feature-layout,
  .feature-system {
    grid-column: span 6;
    min-height: 220px;
  }

  .feature-native {
    grid-column: span 12;
  }
}

@media (max-width: 640px) {
  .features-section {
    padding-bottom: 4rem;
  }

  .feature-native,
  .feature-code,
  .feature-aot,
  .feature-layout,
  .feature-system {
    grid-column: span 12;
    min-height: 200px;
  }
}
</style>
