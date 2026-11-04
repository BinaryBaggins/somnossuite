<script lang="ts">
	import type { ProductionSession } from '$lib/types/ProductionSession.ts';
	import { Button } from '$lib/components/ui/button';
	import { Label } from '$lib/components/ui/label';
	import * as Select from '$lib/components/ui/select/index.js';

	let {
		onSessionStarted
	}: {
		onSessionStarted: (productionSession: ProductionSession) => void;
	} = $props();

	const station = {
		name: 'Grossvieh Linie 1',
		location: 'Schlachthalle A'
	};

	const availableAnimalKinds = [
		{ value: 'Kalb', label: 'Kalb' },
		{ value: 'Rind', label: 'Rind' },
		{ value: 'Kuh', label: 'Kuh' },
		{ value: 'Muni', label: 'Muni' },
		{ value: 'Ochse', label: 'Ochse' }
	];
	let selectedAnimalKind = $state<string>('');

	const triggerContent = $derived(
		availableAnimalKinds.find((f) => f.value === selectedAnimalKind)?.label ??
			'Tierart auswählen'
	);

	function startSession() {
		if (!selectedAnimalKind) {
			return;
		}

		const productionSession: ProductionSession = {
			id: crypto.randomUUID(),
			station: station.name,
			animalKind: selectedAnimalKind,
			startedAt: new Date().toISOString()
		};

		onSessionStarted(productionSession);
	}
</script>

<main class="min-h-screen bg-background p-6 text-foreground">
	<div class="mx-auto grid max-w-4xl gap-6">
		<header class="rounded-3xl border border-border bg-card p-6 text-card-foreground shadow-sm">
			<p class="text-sm font-bold tracking-wider text-primary uppercase">Station</p>

			<h1 class="mt-2 text-4xl font-bold">
				{station.name}
			</h1>

			<p class="mt-2 text-muted-foreground">
				{station.location}
			</p>
		</header>

		<section
			class="rounded-3xl border border-border bg-card p-6 text-card-foreground shadow-sm"
		>
			<h2 class="text-2xl font-bold">Keine aktive Session</h2>

			<p class="mt-2 text-muted-foreground">
				Starten Sie eine neue Produktionssession und wählen Sie die aktuelle Tierart.
			</p>

			<div class="mt-6 grid gap-2 text-xl">
				<Label for="animal-kind-select" class="text-sm font-bold">Aktuelle Tierart</Label>
				<Select.Root
					type="single"
					name="animal-kind-select"
					bind:value={selectedAnimalKind}
				>
					<Select.Trigger class="w-full">
						{triggerContent}
					</Select.Trigger>
					<Select.Content>
						{#each availableAnimalKinds as animal (animal.value)}
							<Select.Item value={animal.value}>
								{animal.label}
							</Select.Item>
						{/each}
					</Select.Content>
				</Select.Root>
			</div>

			<Button
				onclick={startSession}
				disabled={!selectedAnimalKind}
				class="mt-8 h-20 w-full rounded-2xl text-2xl font-bold shadow-sm disabled:cursor-not-allowed disabled:opacity-40"
			>
				Session starten
			</Button>
		</section>

		<footer
			class="rounded-3xl border border-border bg-card p-5 text-sm text-card-foreground shadow-sm"
		>
			<div class="flex justify-between">
				<span class="font-semibold">Benutzer</span>
				<span>Max Muster</span>
			</div>

			<div class="mt-2 flex justify-between">
				<span class="font-semibold">Rolle</span>
				<span>Operator</span>
			</div>
		</footer>
	</div>
</main>
