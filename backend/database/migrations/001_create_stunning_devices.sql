CREATE TABLE
    stunning_devices (
        id uniqueidentifier NOT NULL,
        device_type int NOT NULL,
        manufacturer nvarchar (200) NOT NULL,
        serial_number nvarchar (200) NOT NULL,
        model nvarchar (200) NOT NULL,
        animal_category int NOT NULL,
        last_inspection_date date NULL,
        modified_by_user_id uniqueidentifier NULL,
        modified_at datetimeoffset NULL,
        is_deleted bit NOT NULL CONSTRAINT df_stunning_devices_is_deleted DEFAULT 0,
        CONSTRAINT pk_stunning_devices PRIMARY KEY (id),
        CONSTRAINT uq_stunning_devices_serial_number UNIQUE (serial_number),
        CONSTRAINT ck_stunning_devices_device_type CHECK (device_type IN (0, 1)),
        CONSTRAINT ck_stunning_devices_animal_category CHECK (animal_category IN (0, 1))
    );