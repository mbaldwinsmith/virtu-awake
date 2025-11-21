$dualCombos = @(
  @{ Name = 'BloodlustPsychopath'; Label = 'glitch thrill'; Traits = @('Bloodlust', 'Psychopath'); Text = @('Even gentle stutters read like targets; the thrill was clinical.', 'Clean anomalies felt like puzzles I could take apart without feeling.', 'When the world cracked, I catalogued it and wanted more.') }
  @{ Name = 'PyromaniacTorturedArtist'; Label = 'burned palette'; Traits = @('Pyromaniac', 'TorturedArtist'); Text = @('Little glitch-sparks looked like color trying to escape.', 'Burning seams framed the scene; beauty in ruin felt deliberate.', 'Flame and fractures painted something aching and alive.') }
  @{ Name = 'TooSmartPsychicallySensitive'; Label = 'coded undertone'; Traits = @('TooSmart', 'PsychicallySensitive'); Text = @('I felt the hum and saw the pattern; both lined up.', 'Each ripple had a logic and a mood; I traced both at once.', 'The code and the feeling synced; the seam told me its proof.') }
  @{ Name = 'AsceticBodyPurist'; Label = 'pure signal'; Traits = @('Ascetic', 'BodyPurist'); Text = @('Plain motion, untouched body; nothing extra felt right.', 'Even the glitch edges stayed clean; no ornament clung to me.', 'Pure signal, unaltered skin; the sim finally felt honest.') }
  @{ Name = 'SanguineOptimist'; Label = 'hopeful flicker'; Traits = @('Sanguine', 'Optimist'); Text = @('Little flickers read as good omens.', 'Every stutter felt like encouragement mid-flow.', 'Even big seams looked like openings instead of threats.') }
  @{ Name = 'DepressivePessimist'; Label = 'inevitable crack'; Traits = @('Depressive', 'Pessimist'); Text = @('Small glitches felt like proof the calm would not last.', 'Cracks in the scene matched the sinking feeling.', 'Even at peak focus, I waited for the world to fall through.') }
  @{ Name = 'BrawlerTough'; Label = 'honest impact'; Traits = @('Brawler', 'Tough'); Text = @('Soft hits and stutters still scratched the clash itch.', 'Pressure and lag felt like honest resistance I could push through.', 'When the sim hit hard, I leaned in and kept swinging.') }
  @{ Name = 'MasochistVolatile'; Label = 'welcome hazard'; Traits = @('Masochist', 'Volatile'); Text = @('The sim''s jitter carried a pinch I liked.', 'Every spike of tension landed as fuel.', 'Wild swings and hurt lined up; I rode the edge gladly.') }
  @{ Name = 'CannibalGourmand'; Label = 'hollow feast'; Traits = @('Cannibal', 'Gourmand'); Text = @('Sim flavors were thin; appetite kept its own memory.', 'Perfect dishes tasted hollow; hunger asked for something real.', 'Even in a flawless meal, I craved the forbidden flavor underneath.') }
  @{ Name = 'GreatMemoryNeurotic'; Label = 'catalogued seams'; Traits = @('GreatMemory', 'Neurotic'); Text = @('I logged every hitch; replayed them even while working.', 'Cracks stayed sharp in my mind; I checked them twice.', 'I kept a mental archive of every shimmer and watched for repeats.') }
  @{ Name = 'KindPsychicallySensitive'; Label = 'soft echo'; Traits = @('Kind', 'PsychicallySensitive'); Text = @('I felt the echo''s mood and matched it gently.', 'Every overlap carried feeling; I kept the pace tender.', 'Even in deep focus, I listened for the quiet minds under the glitch.') }
  @{ Name = 'TriggerHappyCarefulShooter'; Label = 'rhythm control'; Traits = @('TriggerHappy', 'CarefulShooter'); Text = @('Quick shots, then a pause; rhythm mattered more than polish.', 'I toggled between bursts and stillness; both felt right.', 'Fast volleys and perfect aims alternated like a pattern I owned.') }
  @{ Name = 'NightOwlUndergrounder'; Label = 'tunnel starlight'; Traits = @('NightOwl', 'Undergrounder'); Text = @('Dim sim, enclosed air; everything felt like night.', 'Depth and hush steadied me; flickers looked like far-off lights.', 'Even big stutters felt like stars in a tunnel sky.') }
  @{ Name = 'BodyModderPsychicallyHypersensitive'; Label = 'invited rewrites'; Traits = @('BodyModder', 'PsychicallyHypersensitive'); Text = @('I sensed shapes my body could grow into; the sim whispered upgrades.', 'Every ripple suggested a new form; the hum made it vivid.', 'Glitches felt like invitations to rewrite myself.') }
  @{ Name = 'JealousGreedy'; Label = 'hoarded glitch'; Traits = @('Jealous', 'Greedy'); Text = @('I kept thinking someone else got more from this than I did.', 'Even good flow felt owed; I wanted the biggest cut of the sim.', 'I watched the seams like a vault, sure there was more hidden.') }
)

$tripleCombos = @(
  @{ Name = 'TooSmartPsychopathSanguine'; Label = 'cheerful dissection'; Traits = @('TooSmart', 'Psychopath', 'Sanguine'); Text = @('I happily mapped the cracks; feelings optional, results bright.', 'Solving the seam felt joyful; empathy never entered.', 'I cheerfully dissected the glitch until it sang.') }
  @{ Name = 'TorturedArtistPsychicallySensitiveDepressive'; Label = 'aching chorus'; Traits = @('TorturedArtist', 'PsychicallySensitive', 'Depressive'); Text = @('Every shimmer sounded sad; I tried to shape it anyway.', 'Glitches carried a music of grief; I followed the tune.', 'I made beauty out of the ache the sim kept whispering.') }
  @{ Name = 'AsceticBodyPuristGreatMemory'; Label = 'archived purity'; Traits = @('Ascetic', 'BodyPurist', 'GreatMemory'); Text = @('I noted each plain, clean motion; stored them untouched.', 'Unadorned steps imprinted clearly; nothing impure stayed.', 'I archived the pure parts and discarded every stray shimmer.') }
  @{ Name = 'PyromaniacVolatileMasochist'; Label = 'bright punishment'; Traits = @('Pyromaniac', 'Volatile', 'Masochist'); Text = @('Little burns and jitters pleased me; risk felt warm.', 'Wild heat and stutter hurt just right; I leaned closer.', 'When the sim flared and bit, it felt perfect.') }
  @{ Name = 'KindOptimistPsychicallySensitive'; Label = 'gentle signal'; Traits = @('Kind', 'Optimist', 'PsychicallySensitive'); Text = @('I felt a gentle hum under everything; I chose to trust it.', 'Even the cracks seemed kind; I met them with warmth.', 'In the deepest flicker, I believed the sim meant well.') }
)

function Write-ComboDefs {
  param(
    [Parameter(Mandatory = $true)] [array] $Combos,
    [Parameter(Mandatory = $true)] [string] $FilePath,
    [Parameter(Mandatory = $true)] [string] $Description,
    [Parameter(Mandatory = $true)] [int[]] $MoodPerTier
  )

  $sb = New-Object System.Text.StringBuilder
  $null = $sb.AppendLine('<?xml version="1.0" encoding="utf-8"?>')
  $null = $sb.AppendLine('<Defs>')

  foreach ($combo in $Combos) {
    for ($i = 0; $i -lt 3; $i++) {
      $tier = $i + 1
      $defName = "VA_VRCombo_$($combo.Name)_T$tier"
      $tierLabel = @('t1', 't2', 't3')[$i]
      $traitsXml = ($combo.Traits | ForEach-Object { "          <li>$_</li>" }) -join "`n"
      $text = $combo.Text[$i]
      $mood = $MoodPerTier[$i]

      $null = $sb.AppendLine('  <ThoughtDef>')
      $null = $sb.AppendLine("    <defName>$defName</defName>")
      $null = $sb.AppendLine("    <label>vr: $($combo.Label) ($tierLabel)</label>")
      $null = $sb.AppendLine('    <durationDays>0.9</durationDays>')
      $null = $sb.AppendLine('    <stackLimit>1</stackLimit>')
      $null = $sb.AppendLine('    <thoughtClass>VirtuAwake.Thought_MemoryTraitVariant</thoughtClass>')
      $null = $sb.AppendLine('    <stages>')
      $null = $sb.AppendLine('      <li>')
      $null = $sb.AppendLine("        <label>$($combo.Label)</label>")
      $null = $sb.AppendLine("        <description>$Description</description>")
      $null = $sb.AppendLine("        <baseMoodEffect>$mood</baseMoodEffect>")
      $null = $sb.AppendLine('      </li>')
      $null = $sb.AppendLine('    </stages>')
      $null = $sb.AppendLine('    <modExtensions>')
      $null = $sb.AppendLine('      <li Class="VirtuAwake.TraitMemoryExtension">')
      $null = $sb.AppendLine('        <variants>')
      $null = $sb.AppendLine('          <li>')
      $null = $sb.AppendLine('            <traits>')
      $null = $sb.AppendLine($traitsXml)
      $null = $sb.AppendLine('            </traits>')
      $null = $sb.AppendLine("            <tier>$tier</tier>")
      $null = $sb.AppendLine("            <text>$text</text>")
      $null = $sb.AppendLine('          </li>')
      $null = $sb.AppendLine('        </variants>')
      $null = $sb.AppendLine('      </li>')
      $null = $sb.AppendLine('    </modExtensions>')
      $null = $sb.AppendLine('  </ThoughtDef>')
    }
  }

  $null = $sb.AppendLine('</Defs>')
  [System.IO.File]::WriteAllText($FilePath, $sb.ToString())
}

Write-ComboDefs -Combos $dualCombos -FilePath 'Defs/ThoughtDefs/Thoughts_VR_Traits_Dual.xml' -Description 'A rare dual-trait VR memory.' -MoodPerTier @(8, 12, 16)
Write-ComboDefs -Combos $tripleCombos -FilePath 'Defs/ThoughtDefs/Thoughts_VR_Traits_Triple.xml' -Description 'A rare triple-trait VR memory.' -MoodPerTier @(10, 14, 18)
