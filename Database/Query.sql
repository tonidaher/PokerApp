
  With RaisePreFlop AS(
  SELECT phc.PlayerName,max(phc.NbHands) as NbHands,count(1) as 'RaisePreflop',cast(count(1) as float)/ cast(max(phc.NbHands) as float) *100.0 as RaisePreflopPct
  FROM [Poker].[dbo].[Join_HandAction] ha
  inner join currentPlayers phc on phc.PlayerName = ha.PlayerName
  inner join Dim_Action a on a.ActionTypeId = ha.ActionTypeId
  inner join Dim_Step s on s.StepId = ha.StepId
  where ActionName in ('Raise','bet') and StepName = 'Preflop'
  group by phc.PlayerName),
  CallPreFlop as(
    SELECT phc.PlayerName,max(phc.NbHands) as NbHands,count(1) as 'CallPreflop',cast(count(1) as float)/ cast(max(phc.NbHands) as float) *100.0 as CallPreflopPct
  FROM [Poker].[dbo].[Join_HandAction] ha
  inner join currentPlayers phc on phc.PlayerName = ha.PlayerName
  inner join Dim_Action a on a.ActionTypeId = ha.ActionTypeId
  inner join Dim_Step s on s.StepId = ha.StepId
  where ActionName in('Call','check') and StepName = 'Preflop'
  group by phc.PlayerName),
    FoldPreFlop as(
    SELECT phc.PlayerName,max(phc.NbHands) as NbHands,count(1) as 'FoldPreflop',cast(count(1) as float)/ cast(max(phc.NbHands) as float) *100.0 as FoldPreflopPct
  FROM [Poker].[dbo].[Join_HandAction] ha
  inner join currentPlayers phc on phc.PlayerName = ha.PlayerName
  inner join Dim_Action a on a.ActionTypeId = ha.ActionTypeId
  inner join Dim_Step s on s.StepId = ha.StepId
  where ActionName = 'Fold' and StepName = 'Preflop'
  group by phc.PlayerName),
    RaiseFlop AS(
  SELECT phc.PlayerName,max(phc.NbHands) as NbHands,count(1) as 'RaiseFlop',cast(count(1) as float)/ cast(max(phc.NbHands) as float) *100.0 as RaiseFlopPct
  FROM [Poker].[dbo].[Join_HandAction] ha
  inner join currentPlayers phc on phc.PlayerName = ha.PlayerName
  inner join Dim_Action a on a.ActionTypeId = ha.ActionTypeId
  inner join Dim_Step s on s.StepId = ha.StepId
  where ActionName in ('Raise','bet') and StepName = 'Flop'
  group by phc.PlayerName),
  CallFlop as(
    SELECT phc.PlayerName,max(phc.NbHands) as NbHands,count(1) as 'CallFlop',cast(count(1) as float)/ cast(max(phc.NbHands) as float) *100.0 as CallFlopPct
  FROM [Poker].[dbo].[Join_HandAction] ha
  inner join currentPlayers phc on phc.PlayerName = ha.PlayerName
  inner join Dim_Action a on a.ActionTypeId = ha.ActionTypeId
  inner join Dim_Step s on s.StepId = ha.StepId
  where ActionName in('Call','check') and StepName = 'Flop'
  group by phc.PlayerName),
    FoldFlop as(
    SELECT phc.PlayerName,max(phc.NbHands) as NbHands,count(1) as 'FoldFlop',cast(count(1) as float)/ cast(max(phc.NbHands) as float) *100.0 as FoldFlopPct
  FROM [Poker].[dbo].[Join_HandAction] ha
  inner join currentPlayers phc on phc.PlayerName = ha.PlayerName
  inner join Dim_Action a on a.ActionTypeId = ha.ActionTypeId
  inner join Dim_Step s on s.StepId = ha.StepId
  where ActionName = 'Fold' and StepName = 'Flop'
  group by phc.PlayerName)
  select cpf.*,rpf.RaisePreflopPct, fpf.FoldPreflopPct,cf.CallFlopPct, rf.RaiseFlopPct, ff.FoldFlopPct
 from CallPreFlop cpf
 full join RaisePreFlop rpf on cpf.PlayerName = rpf.PlayerName
  full join FoldPreFlop fpf on cpf.PlayerName = fpf.playername
  full join CallFlop cf on cf.PlayerName= cpf.PlayerName
  full join RaiseFlop rf on cpf.PlayerName = rf.PlayerName
  full join FoldFlop ff on cpf.PlayerName = ff.PlayerName