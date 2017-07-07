CREATE TABLE [annotations](
    [id] INT NOT NULL, 
    [file_id] INT NOT NULL, 
    [position] DOUBLE NOT NULL, 
    [annotation] VARCHAR(500) NOT NULL);

CREATE TABLE [files](
    [id] VARCHAR(50) NOT NULL, 
    [file_name] VARCHAR(500) NOT NULL, 
    [file_full_name] VARCHAR(500) NOT NULL, 
    [file_hash_code] VARCHAR(100) NOT NULL, 
    [file_hash_type] VARCHAR(20) NOT NULL DEFAULT MD5, 
    [useable] BOOLEAN NOT NULL DEFAULT 1);