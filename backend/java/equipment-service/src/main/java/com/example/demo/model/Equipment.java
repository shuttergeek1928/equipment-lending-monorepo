package com.example.demo.model;

import jakarta.persistence.*;
import jakarta.validation.constraints.Min;
import jakarta.validation.constraints.NotBlank;
import lombok.Getter;
import lombok.NoArgsConstructor;
import lombok.Setter;
import org.hibernate.annotations.CreationTimestamp;

import java.time.LocalDate;
import java.util.UUID;

@Entity
@Table(name = "Equipments")
@Getter
@Setter
@NoArgsConstructor
public class Equipment {

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name = "Id") // Maps to 'Id'
    private UUID id;

    @Column(name = "EquipmentId", unique = true)
    private Integer equipmentId;

    @NotBlank
    @Column(name = "EquipmentName")
    private String equipmentName;

    @NotBlank
    @Column(name = "Catgory")
    private String category;

    @Column(name = "EquipmentCondition")
    private String equipmentCondition;

    @Min(0)
    @Column(name = "TotalQuantity")
    private int totalQuantity;

    @Min(0)
    @Column(name = "AvailableQuantity")
    private int availableQuantity;

    @Column(name = "IsAvailable")
    private boolean isAvailable;

    @CreationTimestamp
    @Column(name = "AddedOn", updatable = false)
    private LocalDate addedOn;
}