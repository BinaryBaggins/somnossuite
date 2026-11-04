<script lang="ts">
	import { LanguageSwitcher } from '#lib/components/ui/language-switcher/index.ts';

	import LL, { locale, setLocale } from '../../../../i18n/i18n-svelte';
	import { loadLocale } from '../../../../i18n/i18n-util.sync';
	import type { Locales } from '../../../../i18n/i18n-types';
	import { persistLocale } from '../../../i18n/locale';

	// The current locale is stored in a Svelte store, so we can subscribe to it
	// and reactively update the component when it changes
	let languages = $derived([
		{ code: 'en', label: $LL.common.localeOptionEn() },
		{ code: 'de', label: $LL.common.localeOptionDe() }
	] satisfies { code: Locales; label: string }[]);

	let value: Locales = $state($locale);

	// Whenever the value changes, we need to load the new locale and set it
	$effect(() => {
		if (value === $locale) return;

		loadLocale(value);
		setLocale(value);
		persistLocale(value);
	});

	// Sync the value with the current locale in case it changes outside of this component
	$effect(() => {
		if (value !== $locale) {
			value = $locale;
		}
	});
</script>

<LanguageSwitcher {languages} bind:value />
