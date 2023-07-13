-- --------------------------------------------------------
-- Host:                         127.0.0.1
-- Server version:               10.10.2-MariaDB - mariadb.org binary distribution
-- Server OS:                    Win64
-- HeidiSQL Version:             11.3.0.6295
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

-- Create a stored procedure to insert a new user into the 'users' table
CREATE PROCEDURE `insUser`(IN `pUsername` VARCHAR(255)) SQL SECURITY INVOKER BEGIN
    INSERT INTO users (`last_name`)
    VALUES (pUsername);
    -- Select the newly inserted user by their ID
    SELECT *
    FROM users u
    WHERE u.id = LAST_INSERT_ID();
END;

-- Create a stored procedure to insert a new user token into the 'user_tokens' table
CREATE PROCEDURE `insUserToken`(IN `pUserId` BIGINT, IN `pToken` VARCHAR(255)) SQL SECURITY INVOKER BEGIN
    INSERT INTO user_tokens (`user_id`, `token`)
    VALUES (pUserId, pToken);
END;

-- Create a stored procedure to insert a new user identity into the 'user_identities' table
CREATE PROCEDURE `insUserIdentity`(
    IN `pUserId` BIGINT,
    IN `pType` VARCHAR(50),
    IN `pValue` VARCHAR(255)
) SQL SECURITY INVOKER BEGIN
    INSERT INTO user_identities (`user_id`, `type`, `identity`)
    VALUES (pUserId, pType, pValue);
END;

-- Create a stored procedure to select a user by their identity from the 'users' table
CREATE PROCEDURE `selUserByIdentity`(
    IN `pType` VARCHAR(50),
    IN `pValue` VARCHAR(255)
) SQL SECURITY INVOKER BEGIN
    SELECT u.*
    FROM user_identities ui
    INNER JOIN users u ON u.id = ui.user_id
    WHERE ui.`type` = pType
    AND ui.identity = pValue;
END;

-- Create a stored procedure to select a user by their token from the 'users' table
CREATE PROCEDURE `selUserByToken`(IN `pTokens` VARCHAR(2000)) SQL SECURITY INVOKER BEGIN
    SELECT u.*
    FROM user_tokens ut
    INNER JOIN users u ON u.id = ut.user_id
    WHERE FIND_IN_SET(ut.token, pTokens);
END;

/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
