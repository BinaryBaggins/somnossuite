import { browser } from '$app/env';
import type { Locales } from '#i18n/i18n-types.ts';

const LOCALE_STORAGE_KEY = 'somnos.locale';
const DEFAULT_LOCALE: Locales = 'en';

const SUPPORTED_LOCALES: Locales[] = ['en', 'de'];

const isLocale = (value: string | null): value is Locales =>
	value !== null && SUPPORTED_LOCALES.includes(value as Locales);

const mapToSupportedLocale = (value: string): Locales => {
	const normalized = value.toLowerCase();
	if (normalized.startsWith('de')) return 'de';
	return DEFAULT_LOCALE;
};

export const getStoredLocale = (): Locales | null => {
	if (!browser) return null;
	const stored = window.localStorage.getItem(LOCALE_STORAGE_KEY);
	return isLocale(stored) ? stored : null;
};

export const detectBrowserLocale = (): Locales => {
	if (!browser) return DEFAULT_LOCALE;
	const firstPreferred = navigator.languages?.[0] ?? navigator.language;
	return mapToSupportedLocale(firstPreferred);
};

export const getPreferredLocale = (): Locales => getStoredLocale() ?? detectBrowserLocale();

export const persistLocale = (locale: Locales): void => {
	if (!browser) return;
	window.localStorage.setItem(LOCALE_STORAGE_KEY, locale);
};

export const defaultLocale = DEFAULT_LOCALE;
