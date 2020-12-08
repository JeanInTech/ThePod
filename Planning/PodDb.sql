CREATE DATABASE PodDb;
USE PodDb;

CREATE TABLE UserFeedback(
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	UserId NVARCHAR(450) FOREIGN KEY REFERENCES AspNetUsers(Id) NOT NULL,
	EpisodeId NVARCHAR(50) NOT NULL,
	Tags NVARCHAR(150),
	Rating TINYINT,
	Review NVARCHAR(MAX)
);

CREATE TABLE SavedPodcast(
	Id INT PRIMARY KEY IDENTITY(1,1) NOT NULL,
	UserId NVARCHAR(450) FOREIGN KEY REFERENCES AspNetUsers(Id) NOT NULL,
	EpisodeId NVARCHAR(50) NOT NULL,
	PodcastName NVARCHAR(50) NOT NULL,
	EpisodeName NVARCHAR(100) NOT NULL, 
	Publisher NVARCHAR(50),
	[Description] NVARCHAR(MAX), 
	AudioPreviewURL NVARCHAR(MAX),
	ExternalURLs NVARCHAR(MAX),
	ImageUrl NVARCHAR(MAX), --figure out how to get a single image from array
	Duration INT,
	ReleaseDate DATE --need to convert from string
);
