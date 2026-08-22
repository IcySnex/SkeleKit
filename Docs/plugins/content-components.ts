import LandingFeatures from '~/components/LandingFeatures.vue';
import LandingHero from '~/components/LandingHero.vue';
import DocsProseH1 from '~/internal/components/DocsProseH1.vue';
import DocsProseH2 from '~/internal/components/DocsProseH2.vue';
import DocsProseP from '~/internal/components/DocsProseP.vue';

export default defineNuxtPlugin((nuxtApp) => {
  nuxtApp.vueApp.component('LandingFeatures', LandingFeatures);
  nuxtApp.vueApp.component('LandingHero', LandingHero);
  nuxtApp.vueApp.component('ProseH1', DocsProseH1);
  nuxtApp.vueApp.component('ProseH2', DocsProseH2);
  nuxtApp.vueApp.component('ProseP', DocsProseP);
});
