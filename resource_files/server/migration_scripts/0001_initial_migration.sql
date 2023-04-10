CREATE PROCEDURE `insUser`(IN `pUsername` VARCHAR(255)) SQL SECURITY INVOKER BEGIN
INSERT INTO users (`last_name`)
VALUES (pUsername);
SELECT *
FROM users u
WHERE u.id = LAST_INSERT_ID();
END;
CREATE PROCEDURE `insUserToken`(IN `pUserId` BIGINT, IN `pToken` VARCHAR(255)) SQL SECURITY INVOKER BEGIN
INSERT INTO user_tokens (`user_id`, `token`)
VALUES (pUserId, pToken);
END;
CREATE PROCEDURE `insUserIdentity`(
  IN `pUserId` BIGINT,
  IN `pType` VARCHAR(50),
  IN `pValue` VARCHAR(255)
) SQL SECURITY INVOKER BEGIN
INSERT INTO user_identities (`user_id`, `type`, `identity`)
VALUES (pUserId, pType, pValue);
END;
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
CREATE PROCEDURE `selUserByToken`(IN `pTokens` VARCHAR(2000)) SQL SECURITY INVOKER BEGIN
SELECT u.*
FROM user_tokens ut
  INNER JOIN users u ON u.id = ut.user_id
WHERE FIND_IN_SET(ut.token, pTokens);
END;