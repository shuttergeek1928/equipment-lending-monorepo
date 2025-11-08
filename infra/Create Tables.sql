CREATE TABLE IF NOT EXISTS public.UserTypes(
	TypeId SMALLSERIAL PRIMARY KEY ,
	TypeValue varchar(100) UNIQUE NOT NULL
);

CREATE TABLE IF NOT EXISTS public.Users(
	ID UUID PRIMARY KEY,
	UserId SERIAL UNIQUE NOT NULL,
	UserName varchar(255) NOT NULL,
	Email varchar(255) UNIQUE NOT NULL,
	PasswordHash varchar(255) NOT NULL,
	PasswordSalt varchar(255)NOT NULL,
	UserType INT NOT NULL REFERENCES UserTypes(TypeId),
	IsActive BOOLEAN NOT NULL DEFAULT FALSE,
	IsDeleted BOOLEAN NOT NULL DEFAULT FALSE,
	CreatedAt TIMESTAMPTZ NOT NULL DEFAULT NOW(),
	UpdatedAt TIMESTAMPTZ,
	LastAccessedAt TIMESTAMPTZ
);

CREATE TABLE IF NOT EXISTS public.Equipments(
	ID UUID PRIMARY KEY,
	EquipmentId SERIAL UNIQUE,
	EquipmentName varchar(255) NOT NULL,
	Catgory varchar(255),
	EquipmentCondition varchar(255),
	TotalQuantity INT NOT NULL DEFAULT 1,
	AvailableQuantity INT NOT NULL DEFAULT 1,
	IsAvailable BOOLEAN DEFAULT TRUE,
	AddedOn DATE NOT NULL DEFAULT CURRENT_DATE
);

CREATE TABLE IF NOT EXISTS public.BorrowingsAndReturns(
	ID UUID PRIMARY KEY,
	EquipmentId INT NOT NULL,
	RequestId SERIAL UNIQUE,
	RequesterId INT NOT NULL,
	RequestedQuantity INT NOT NULL CHECK(RequestedQuantity > 0),
	RequestedOn TIMESTAMPTZ,
	ReturnDueDate TIMESTAMPTZ,
	ReturnedOn TIMESTAMPTZ,
	IsApproved BOOLEAN DEFAULT FALSE,
	IsReturned BOOLEAN DEFAULT FALSE,
	Notes varchar(255),

	-- Corrected foreign keys:
	CONSTRAINT fk_bnr_equipment
		FOREIGN KEY (EquipmentId) REFERENCES public.equipments(EquipmentId)
		ON UPDATE CASCADE,

	CONSTRAINT fk_bnr_requester
		FOREIGN KEY (RequesterId) REFERENCES public.users(UserId)
		ON UPDATE CASCADE
		ON DELETE RESTRICT
);

