
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 01/27/2023 16:12:57
-- Generated from EDMX file: C:\Users\Jiga\Downloads\Browser\Browser\Browser\Browser\Db.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [BrowserDb];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_Bookmark_User]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Bookmark] DROP CONSTRAINT [FK_Bookmark_User];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Bookmark]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Bookmark];
GO
IF OBJECT_ID(N'[dbo].[User]', 'U') IS NOT NULL
    DROP TABLE [dbo].[User];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Bookmarks'
CREATE TABLE [dbo].[Bookmarks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [URL] nvarchar(max)  NULL,
    [Title] nvarchar(max)  NULL,
    [UserId] int  NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserName] nvarchar(50)  NULL,
    [Email] nvarchar(50)  NULL,
    [Password] nvarchar(50)  NULL,
    [IsLogIn] bit  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Bookmarks'
ALTER TABLE [dbo].[Bookmarks]
ADD CONSTRAINT [PK_Bookmarks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [UserId] in table 'Bookmarks'
ALTER TABLE [dbo].[Bookmarks]
ADD CONSTRAINT [FK_Bookmark_User]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Bookmark_User'
CREATE INDEX [IX_FK_Bookmark_User]
ON [dbo].[Bookmarks]
    ([UserId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------