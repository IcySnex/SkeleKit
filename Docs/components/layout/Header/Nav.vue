<template>
  <UiNavigationMenu>
    <UiNavigationMenuList>
      <UiNavigationMenuItem v-for="(item, i) in nav" :key="i" class="relative">
        <template v-if="item.links">
          <UiNavigationMenuTrigger class="bg-transparent text-sm font-medium">
            {{ $t(item.title) }}
          </UiNavigationMenuTrigger>
          <UiNavigationMenuContent>
            <ul class="w-[250px] p-2">
              <li v-for="link in item.links" :key="link.title">
                <NuxtLinkLocale
                  :to="link.to && localePath(link.to)"
                  :target="link.target"
                  class="hover:bg-muted mb-1 flex w-full gap-2 rounded-md px-3 py-2 transition-all"
                >
                  <SmartIcon v-if="link?.icon" :name="link.icon" :size="16" class="mt-1 min-w-5" />
                  <div>
                    <div v-if="link.title" class="text-sm font-medium">{{ $t(link.title) }}</div>
                    <div v-if="link.description" class="text-muted-foreground text-xs leading-5">{{ $t(link.description) }}</div>
                  </div>
                </NuxtLinkLocale>
              </li>
            </ul>
          </UiNavigationMenuContent>
        </template>
        <NuxtLinkLocale v-else :to="localePath(item.to)" :target="item.target">
          <Icon v-if="item.showLinkIcon ?? true" name="lucide:arrow-up-right" class="text-muted-foreground absolute right-2 top-2" size="13" />
          <div
            class="hover:bg-accent hover:text-accent-foreground focus:bg-accent focus:text-accent-foreground inline-flex h-10 w-max items-center justify-center rounded-md bg-transparent px-4 py-2 text-sm font-medium transition-colors focus:outline-none"
            :class="(item.showLinkIcon ?? true) && 'pr-6'"
          >
            {{ $t(item.title) }}
          </div>
        </NuxtLinkLocale>
      </UiNavigationMenuItem>
    </UiNavigationMenuList>
  </UiNavigationMenu>
</template>

<script setup lang="ts">
const { nav } = useConfig().value.header;
const { localePath } = useI18nDocs();
</script>
