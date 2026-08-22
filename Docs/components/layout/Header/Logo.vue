<template>
  <div>
    <NuxtLinkLocale v-if="logo.light && logo.dark" :to="localePath('/')" class="flex items-center">
      <NuxtImg
        :src="logo.light"
        width="28"
        height="28"
        alt="SkeleKit"
        class="block size-7 object-contain dark:hidden"
      />
      <NuxtImg
        :src="logo.dark"
        width="28"
        height="28"
        alt="SkeleKit"
        class="hidden size-7 object-contain dark:block"
      />
      <span v-if="showTitle && title" class="ml-3 self-center font-semibold">
        {{ $t(title) }}
      </span>
    </NuxtLinkLocale>
  </div>
</template>

<script setup lang="ts">
const { logo, title, showTitle } = useConfig().value.header;
const { localePath } = useI18nDocs();
</script>
