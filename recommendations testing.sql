--/****** Script for SelectTopNRows command from SSMS  ******/
--SELECT TOP (1000) [Id]
--      ,[UserFeedbackId]
--      ,[UserId]
--      ,[EpisodeId]
--      ,[Tag]
--      ,[Rating]
--  FROM [UserProfile]
--DELETE FROM USERPROFILE WHERE UserFeedbackId IS NULL

--SELECT * FROM UserProfile 
	  --THIS QUERY IS GETTING ME MY PROFILE METRICS TO CREATE A BASELINE OF WHAT A SPECIFIC USER LIKES
	  --SELECT COUNT (Tag) AS Tag_Count, SUM (Rating) AS Rating_Sum, Tag, [Id] FROM UserProfile WHERE UserId = '0a3a2c18-01b6-44a2-a23b-5901db9cefc1' GROUP BY Tag ORDER BY Rating_Sum DESC;
	  --THIS ONE IS NO GOOD- IT JUST GIVES ME THE TOP RATED TAGS
	  --SELECT COUNT (Tag) AS 'Tag Count', SUM (Rating) AS 'Rating total', Tag FROM UserProfile WHERE Rating >= 3 AND UserId <> '0a3a2c18-01b6-44a2-a23b-5901db9cefc1'  GROUP BY Tag
	  --THE QUERY BELOW IS USING THE USER PROFILE TABLE TO MATCH RESULTS THAT ARENT MINE
	  --SELECT EpisodeId, Tag, Rating FROM UserProfile WHERE UserId <> '0a3a2c18-01b6-44a2-a23b-5901db9cefc1' ORDER BY Rating DESC

	  --THIS IS ME TRYING TO GET A LIST OF POTENTIAL MATCHES FROM THE FEEDBACK TABLE - ITS NOT GOING TO WORK BECAUSE TAGS ARE CSV
--SELECT * FROM UserFeedback
	  --SELECT COUNT (Tags) AS 'Tag Count', SUM (Rating) AS 'Rating total', Tags FROM UserFeedback WHERE Rating >= 3 AND UserId <> '0a3a2c18-01b6-44a2-a23b-5901db9cefc1'  GROUP BY Tags

	 --TRYING TO DO A SELF JOIN:
	 --SELECT A.Tag AS Tag1, B.Tag AS Tag2, A.EpisodeId, A.Rating
	 --FROM UserProfile A, UserProfile B
	 --WHERE A.Tag <> B.Tag
	 --AND A.EpisodeId = B.EpisodeId
	 --ORDER BY A.Rating

	 	  SELECT COUNT (Tag) AS Tag_Count, SUM (Rating) AS Rating_Sum, Tag, [Id], EpisodeId, Rating, UserId, UserFeedbackId FROM UserProfile WHERE UserId = '0a3a2c18-01b6-44a2-a23b-5901db9cefc1' GROUP BY Tag, [Id], Rating, EpisodeId, UserId, UserFeedbackId;