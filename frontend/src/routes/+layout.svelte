<script lang="ts">
	import '../app.css';
	import { browser } from '$app/env';
	import { setLocale } from '../i18n/i18n-svelte';
	import { loadLocale } from '../i18n/i18n-util.sync';
	import { defaultLocale, getPreferredLocale, persistLocale } from '#lib/i18n/locale.ts';
	import favicon from '#lib/assets/favicon.svg';
	import NavBar from '#lib/components/NavBar.svelte';

	const initialLocale = browser ? getPreferredLocale() : defaultLocale;

	loadLocale(initialLocale);
	setLocale(initialLocale);

	if (browser) {
		persistLocale(initialLocale);
	}

	let { children } = $props();
</script>

<svelte:head>
	<link rel="icon" href={favicon} />
</svelte:head>

<NavBar></NavBar>

{@render children?.()}
