-- Supprimer la base de données si elle existe déjà
DROP DATABASE IF EXISTS bdEnquete;
-- Créer la base de données
CREATE DATABASE IF NOT EXISTS bdEnquete;
-- Utiliser la base de données nouvellement créée
USE bdEnquete;

CREATE TABLE Questionnaire(
    cle VARCHAR(12),
    name VARCHAR(40),
    displayName VARCHAR(60),
    description VARCHAR(124),
    PRIMARY KEY(cle)
);

CREATE TABLE Reponses(
    id INT(3) auto_increment,
    cle_questionnaire VARCHAR(12),
    rang INT(2),
    date_creation DATETIME,
    reponse VARCHAR(250),
    
    PRIMARY KEY(id),
    FOREIGN KEY (cle_questionnaire) REFERENCES Questionnaire(cle)
);

DELIMITER //
DROP PROCEDURE IF EXISTS AjouterReponse;
CREATE PROCEDURE AjouterReponse(IN p_cle_questionnaire VARCHAR(12), IN p_reponse VARCHAR(250))
BEGIN
    DECLARE v_rang INT;

    -- Trouver le prochain rang pour ce questionnaire
    SELECT COALESCE(MAX(rang), 0) + 1 INTO v_rang
    FROM Reponses
    WHERE cle_questionnaire = p_cle_questionnaire;

    -- Insérer la réponse avec le rang calculé
    INSERT INTO Reponses (p_cle_questionnaire, rang, date_creation, p_reponse)
    VALUES (p_cle_questionnaire, v_rang, NOW(), reponse);
END//

DELIMITER ;

select * from reponses;