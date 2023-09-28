PRINT N'Creating GMap and GMap.Marker...';  

GO  

CREATE DATABASE GMap

GO

USE GMap

GO

CREATE TABLE Marker (  
    [MarkerID]   INT           IDENTITY (1, 1) NOT NULL,  
    [MarkerName] NVARCHAR (40) NOT NULL,  
    [Latitude]   FLOAT         NOT NULL,  
    [Longitude]  FLOAT         NOT NULL,
    PRIMARY KEY(MarkerID)
);  

GO

CREATE PROCEDURE newMarker 
    @MarkerName NVARCHAR (40),  
    @Latitude FLOAT,
    @Longitude FLOAT,
    @MarkerID INT OUTPUT  
AS 
BEGIN  
    INSERT INTO Marker (MarkerName, Latitude, Longitude) 
    VALUES (@MarkerName, @Latitude, @Longitude)
    SET @MarkerID = SCOPE_IDENTITY()
RETURN @@ERROR  
END 

GO

CREATE PROCEDURE deleteMarker 
    @MarkerID INT  
AS 
BEGIN  
    DELETE Marker 
    WHERE MarkerID = @MarkerID
END 

GO

CREATE PROCEDURE updateMarker 
    @MarkerName NVARCHAR (40),  
    @Latitude FLOAT,
    @Longitude FLOAT,
    @MarkerID INT
AS  
BEGIN 
    UPDATE Marker
    SET MarkerName = @MarkerName, Latitude = @Latitude, Longitude = @Longitude
    WHERE MarkerID = @MarkerID;
END

GO 

CREATE PROCEDURE getAllMarkers  
AS  
BEGIN  
    SELECT * FROM Marker OUTPUT
END  

GO

INSERT Marker (MarkerName, Latitude, Longitude)
    VALUES 
        (N'Октябрьский',	59.489726035537075,	79.398193359375),
        (N'Норильск',    69.3570863282203,   88.17626953125),
        (N'Билибино',	68.05688884134598,	166.44287109375),
        (N'Юкагир',	    71.7739410364347,	139.85595703125),
        (N'Тюмяти',  	71.890409428423325,	123.64013671875),
        (N'Новорыбная',	72.822564132293309,	105.8203125),
        (N'Helsinki',	60.17430626192602,	24.93896484375)

GO

PRINT N'Done';  