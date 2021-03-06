Use [Animal_Movement]
Go

-- Test All Views 


-- Get all the Collar data embeded in the uploaded TPF files
-- Used by multiple queries in external files
SELECT TOP 100 * FROM AllTpfFileData

-- Used by AnimalMovement/FileDetailsForm.cs via the SQL to Linq DataModel
SELECT TOP 10 * FROM AnimalFixesByFile

-- used by ArgosProcessor/Program.cs to determine which files to process 
SELECT TOP 100 * FROM ArgosFile_NeedsPartialProcessing

-- used by ArgosProcessor/Program.cs to determine which files to process 
SELECT TOP 100 * FROM ArgosFile_NeverProcessed

-- used by ArgosProcessor/Program.cs to determine which files to process 
SELECT TOP 100 * FROM DataLog_NeverProcessed

--Spatial layers
SELECT TOP 100 * FROM Gen3StoreOnBoardLocations
SELECT TOP 100 * FROM InvalidLocations
SELECT TOP 100 * FROM LastLocationOfKnownMortalities
SELECT TOP 100 * FROM MostRecentLocations
SELECT TOP 100 * FROM NoMovement
SELECT TOP 100 * FROM ValidLocations
SELECT TOP 100 * FROM VelocityVectors
