
CREATE proc [dbo].FixExactTimeForDbmOfCell_v3
	@operatorId nvarchar(max),
	@jobId nvarchar(max)
as
begin
	WITH TransformTime as (
	 SELECT DISTINCT cdbm.OperatorId,cdbm.JobId,cdbm.CreatedDateUtc,
	 DATEADD(minute,15 * (datediff(second,osr.StartTimeUtc,cdbm.[CreatedDateUtc])-60), osr.OriginalStartTimeUtcKpi) as RealDateTime
	 FROM CellDbms cdbm 
	 JOIN OndemandSaRequest osr on cdbm.OperatorId = osr.Id  and cdbm.JobId = osr.JobId 
	 WHERE cdbm.OperatorId = @operatorId and cdbm.JobId = @jobId
	 )

	   UPDATE cdbm  
	 Set cdbm.CreatedDateUtc = tt.RealDateTime 
	 FROM CellDbms as cdbm
	 JOIN TransformTime as tt 
	 on cdbm.OperatorId = tt.OperatorId
	 and cdbm.JobId = tt.JobId
	 and cdbm.CreatedDateUtc = tt.CreatedDateUtc
	 WHERE cdbm.OperatorId =  @operatorId and cdbm.JobId = @jobId

END


--------------------------------------------------------------------------


CREATE  proc [dbo].[PreProcessOndemandLogEvent_v3]
	@operatorId nvarchar(max),
	@jobId nvarchar(max)
as
begin
	--delete where slogan = MIMO and TXRXMODE !=2
  DELETE FROM OndemandLogEvent
  Where Not exists (
  SELECT *
  FROM   OndemandConfiguration oc
  WHERE OndemandLogEvent.OperatorId = oc.OperatorId
  and OndemandLogEvent.JobId = oc.JobId
  and OndemandLogEvent.CellName = oc.CellName
  and OndemandLogEvent.DateWithoutTimeUtc = oc.CreatedDateUtc
  and oc.TxRxMode = 2
  )
  and OndemandLogEvent.OperatorExecuteCode = 1
  and OndemandLogEvent.OperatorId = @operatorId
  and OndemandLogEvent.JobId = @jobId
  --2nd,3rd rule,

   DELETE FROM OndemandLogEvent 
  Where Not exists (
  SELECT *
  FROM   OndemandConfiguration oc
  WHERE OndemandLogEvent.OperatorId = oc.OperatorId
  and  OndemandLogEvent.JobId = oc.JobId
  and OndemandLogEvent.CellName = oc.CellName
  and OndemandLogEvent.DateWithoutTimeUtc = oc.CreatedDateUtc
  and oc.AllPwrDifferent = 1
  )
  and (OndemandLogEvent.OperatorExecuteCode = 3 or OndemandLogEvent.OperatorExecuteCode = 9)
  and OndemandLogEvent.OperatorId = @operatorId
  and OndemandLogEvent.JobId = @jobId

  DELETE FROM OndemandLogEvent 
  Where Not exists (
  SELECT *
  FROM   OndemandConfiguration oc
  WHERE OndemandLogEvent.OperatorId = oc.OperatorId
  and  OndemandLogEvent.JobId = oc.JobId
  and OndemandLogEvent.CellName = oc.CellName
  and OndemandLogEvent.DateWithoutTimeUtc = oc.CreatedDateUtc
  and oc.AllAttenDifferent = 1
  )
  and (OndemandLogEvent.OperatorExecuteCode = 5 or OndemandLogEvent.OperatorExecuteCode = 11)
  and OndemandLogEvent.OperatorId = @operatorId
    and OndemandLogEvent.JobId = @jobId

    DELETE FROM OndemandLogEvent 
  Where Not exists (
  SELECT *
  FROM   OndemandConfiguration oc
  WHERE OndemandLogEvent.OperatorId = oc.OperatorId
   and  OndemandLogEvent.JobId = oc.JobId
 
  and OndemandLogEvent.CellName = oc.CellName
  and OndemandLogEvent.DateWithoutTimeUtc = oc.CreatedDateUtc
  and oc.AllAttenSame = 1
  and oc.AllPwrSame = 1
  )
  and (OndemandLogEvent.OperatorExecuteCode = 7 or OndemandLogEvent.OperatorExecuteCode = 13)
  and OndemandLogEvent.OperatorId = @operatorId
    and OndemandLogEvent.JobId = @jobId

  -----------------------------------------------------------------------------------------

  -----------------------------UPDATE Addition info for 3,9 by PWR -------------------------
  UPDATE OndemandLogEvent 
  SET OndemandLogEvent.AdditionInfo = OndemandConfiguration.AdditionInfoForPwr
  FROM OndemandLogEvent 
  JOIN OndemandConfiguration
  ON  OndemandLogEvent.OperatorId = OndemandConfiguration.OperatorId
  and OndemandLogEvent.JobId = OndemandConfiguration.JobId
  and OndemandLogEvent.CellName = OndemandConfiguration.CellName
  and OndemandLogEvent.DateWithoutTimeUtc = OndemandConfiguration.CreatedDateUtc
  WHERE OndemandLogEvent.OperatorExecuteCode = 3 or OndemandLogEvent.OperatorExecuteCode = 9 
  


  -----------------------------UPDATE Addition info for 5,11 by ATTEN-------------------------
    UPDATE OndemandLogEvent 
  SET OndemandLogEvent.AdditionInfo = OndemandConfiguration.AdditionInfoForAtten
  FROM OndemandLogEvent 
  JOIN OndemandConfiguration
  ON  OndemandLogEvent.OperatorId = OndemandConfiguration.OperatorId
  and OndemandLogEvent.JobId = OndemandConfiguration.JobId
  and OndemandLogEvent.CellName = OndemandConfiguration.CellName
  and OndemandLogEvent.DateWithoutTimeUtc = OndemandConfiguration.CreatedDateUtc
  WHERE OndemandLogEvent.OperatorExecuteCode = 5 or OndemandLogEvent.OperatorExecuteCode = 11 
  -----------------------------------------------------------------------------------------



   -----------------------------UPDATE EVENT TIME OF 15,16,17 VSWR-------------------------
   ;WITH TransformTimeForVswr as (
	 SELECT DISTINCT ole.OperatorId,ole.CellName,ole.EventTime,ole.DateWithoutTimeUtc,osr.StartTimeUtc,
	 DATEADD(minute,(datediff(millisecond,osr.StartTimeUtc,ole.EventTime)-60000)*15/1000, osr.OriginalStartTimeUtcNetworkAlarm) as RealDateTime,
	ole.EpochMillisecond,ole.OperatorExecuteCode
	 FROM OndemandLogEvent ole 
	 JOIN OndemandSaRequest osr 
	 on
	  ole.OperatorId = osr.Id  
	  and ole.JobId = osr.JobId  
	 WHERE  (ole.OperatorExecuteCode = 15 or ole.OperatorExecuteCode = 16	or ole.OperatorExecuteCode = 17) 
	 and ole.OperatorId = @operatorId
	 and ole.JobId = @jobId
	 
	 )
		 
	UPDATE ole  
	 Set ole.EventTime = tt.RealDateTime 
	 FROM OndemandLogEvent ole
	 JOIN TransformTimeForVswr as tt 
	 on ole.OperatorId = tt.OperatorId
	 and ole.CellName = tt.CellName
	 and ole.EventTime = tt.EventTime
	 WHERE  (ole.OperatorExecuteCode = 15 or ole.OperatorExecuteCode = 16	or ole.OperatorExecuteCode = 17) 
	 and ole.OperatorId = @operatorId
	 and ole.JobId = @jobId
  -----------------------------------------------------------------------------------------




    SELECT CellName,Slogan,EventTime,MoClass,OperatorExecuteCode,
	checkValue = CASE 
				WHEN		LAG(CellName, 1,0) OVER (ORDER BY CellName,Slogan,EventTime) = CellName
					 And	LAG(OperatorExecuteCode, 1,0) OVER (ORDER BY CellName,Slogan,EventTime)  = OperatorExecuteCode
					 And	LAG(Slogan, 1,0) OVER (ORDER BY CellName,Slogan,EventTime)  = Slogan
					
				THEN 1 
				ELSE 0 
				end
	INTO LogEventProcess
	FROM [OndemandLogEvent]
	WHERE OperatorId= @operatorId and JobId = @jobId
	order by CellName,Slogan,EventTime asc
	-----------------------------------------------------------------------
	SELECT ole.* FROM [OndemandLogEvent] ole 
	JOIN LogEventProcess lep
	on ole.Slogan = lep.Slogan and ole.CellName= lep.CellName and ole.EventTime=lep.EventTime 		
	Where lep.checkValue= 0
	and ole.OperatorId = @operatorId and ole.JobId = @jobId
	order by CellName,Slogan,EventTime asc

	DROP TABLE LogEventProcess
	
END