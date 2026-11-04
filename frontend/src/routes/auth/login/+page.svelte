<script lang="ts">
	import { resolve } from '$app/paths';
	import LL from '#i18n/i18n-svelte.js';

	import { Card } from '#lib/components/ui/card/index.ts';
	import { Button } from '#lib/components/ui/button/index.ts';
	import { Input } from '#lib/components/ui/input/index.ts';
	import { Label } from '#lib/components/ui/label/index.ts';
	import { Checkbox } from '#lib/components/ui/checkbox/index.ts';
	import { Badge } from '#lib/components/ui/badge/index.ts';

	import favicon from '#lib/assets/favicon.svg';
</script>

<svelte:head>
	<title>{$LL.login.title()}</title>
</svelte:head>

<main
	class="relative min-h-screen overflow-hidden bg-background sm:px-6 sm:py-8 [@media(max-height:800px)]:py-3"
>
	<!--
	<div class="absolute right-4 top-4 z-20 flex items-center gap-2 sm:right-6 sm:top-6">
		<ThemeToggle />
		<LanguageSwitcher />
	</div>
	-->

	<section
		aria-label="SomnosSuite Login"
		class="relative mx-auto my-auto grid w-full max-w-6xl items-center gap-6 lg:grid-cols-[1.05fr_minmax(320px,460px)] [@media(max-height:800px)]:gap-4"
	>
		<!-- Decorative background -->
		<div
			class="pointer-events-none absolute top-1/2 left-[35%] h-192 w-3xl -translate-x-1/2 -translate-y-1/2 rounded-full bg-primary/15 blur-[120px] dark:bg-primary/10"
		></div>

		<img
			src={favicon}
			aria-hidden="true"
			alt=""
			class="pointer-events-none absolute top-1/2 left-[35%] h-[80vh] w-[80vh] -translate-x-1/2 -translate-y-1/2 opacity-[0.07] dark:opacity-[0.05]"
		/>
		<!-- left-side hero/content section -->
		<div
			class="grid content-center gap-4 px-1 py-2 lg:p-6 [@media(max-height:800px)]:gap-2 [@media(max-height:800px)]:py-1"
		>
			<p class="m-0 text-xs font-bold tracking-[0.18em] text-primary uppercase">
				{$LL.login.applicationName()}
			</p>
			<h1
				class="m-0 max-w-[19ch] font-serif text-4xl leading-[0.95] text-foreground sm:text-5xl lg:text-6xl [@media(max-height:800px)]:lg:text-5xl"
			>
				{$LL.login.headline()}
			</h1>
			<p class="m-0 max-w-2xl text-base leading-relaxed text-foreground/75 sm:text-lg">
				{$LL.login.subline()}
			</p>

			<ul class="mt-3 grid gap-3">
				<li class="flex items-center gap-3 text-sm text-foreground/75">
					<span
						aria-hidden="true"
						class="h-2.5 w-2.5 shrink-0 rounded-full bg-primary ring-4 ring-primary/20"
					></span>
					<span>{$LL.login.benefitSecure()}</span>
				</li>
				<li class="flex items-center gap-3 text-sm text-foreground/75">
					<span
						aria-hidden="true"
						class="h-2.5 w-2.5 shrink-0 rounded-full bg-primary ring-4 ring-primary/20"
					></span>
					<span>{$LL.login.benefitDashboards()}</span>
				</li>
				<li class="flex items-center gap-3 text-sm text-foreground/75">
					<span
						aria-hidden="true"
						class="h-2.5 w-2.5 shrink-0 rounded-full bg-primary ring-4 ring-primary/20"
					></span>
					<span>{$LL.login.benefitTriage()}</span>
				</li>
			</ul>
		</div>

		<!-- Sign in form -->
		<div class="grid items-center">
			<Card
				aria-label="Sign in form"
				class="rounded-3xl border-border/50 bg-card/65 p-5 shadow-2xl backdrop-blur-xl supports-backdrop-filter:bg-card/55 sm:p-7"
			>
				<header>
					<div class="mb-3 flex items-center justify-between gap-3">
						<Badge
							title="Current environment"
							class="inline-flex items-center rounded-full px-2.5 py-1 text-[11px] font-bold tracking-wide uppercase"
						>
							{$LL.login.envLabel()}
						</Badge>
						<Badge
							variant="secondary"
							class="inline-flex items-center rounded-full px-2.5 py-1 text-[11px] font-bold tracking-wide uppercase"
						>
							{$LL.login.buildLabel()}
						</Badge>
					</div>
					<h2 class="m-0 font-serif text-4xl text-foreground">
						{$LL.login.signInHeading()}
					</h2>
					<p class="mt-1 mb-5 text-sm text-foreground/75">
						{$LL.login.signInSubline()}
					</p>
				</header>

				<form method="post" action="/login" class="grid gap-2.5">
					<Label for="email" class="text-xs font-bold tracking-wide text-foreground/75">
						{$LL.login.workEmailLabel()}
					</Label>
					<Input
						id="email"
						name="email"
						type="email"
						autocomplete="email"
						placeholder={$LL.login.workEmailPlaceholder()}
						class="h-11 rounded-xl border-border/70 bg-background/70 px-3 text-sm shadow-sm"
					/>

					<Label
						for="password"
						class="text-xs font-bold tracking-wide text-foreground/75"
					>
						{$LL.login.passwordLabel()}
					</Label>
					<Input
						id="password"
						type="password"
						name="password"
						autocomplete="current-password"
						placeholder={$LL.login.passwordPlaceholder()}
						required
						class="h-11 rounded-xl border-border/70 bg-background/70 px-3 text-sm shadow-sm"
					/>

					<div class="mt-1 mb-1 flex items-center justify-between gap-3">
						<Label
							for="remember"
							class="inline-flex items-center gap-2 text-xs font-semibold text-foreground/80"
						>
							<Checkbox
								id="remember"
								name="remember"
								class="h-4 w-4 border-border/70"
							/>
							<span>{$LL.login.rememberDevice()}</span>
						</Label>
						<a
							href={resolve('/about')}
							class="text-xs font-semibold text-primary hover:text-primary/80 hover:underline"
						>
							{$LL.login.forgotPassword()}
						</a>
					</div>

					<Button
						type="submit"
						class="mt-1 h-11 rounded-xl text-sm font-bold shadow-sm transition-transform hover:-translate-y-0.5"
					>
						{$LL.login.signInButton()}
					</Button>
				</form>

				<div
					aria-hidden="true"
					class="my-4 grid grid-cols-[1fr_auto_1fr] items-center gap-3"
				>
					<span class="h-px bg-border"></span>
					<p
						class="m-0 text-[11px] font-bold tracking-[0.09em] text-muted-foreground uppercase"
					>
						{$LL.login.orLabel()}
					</p>
					<span class="h-px bg-border"></span>
				</div>

				<Button
					type="button"
					variant="outline"
					class="h-11 w-full rounded-xl bg-background/40"
				>
					{$LL.login.ssoButton()}
				</Button>

				<footer class="mt-4 flex flex-wrap items-center justify-between gap-2">
					<a
						href={resolve('/about')}
						class="text-xs font-semibold text-primary hover:text-primary/80 hover:underline"
					>
						{$LL.login.needAccess()}
					</a>
					<a
						href={resolve('/about')}
						class="text-xs font-semibold text-primary hover:text-primary/80 hover:underline"
					>
						{$LL.login.systemStatus()}
					</a>
				</footer>
			</Card>
		</div>
	</section>
</main>
