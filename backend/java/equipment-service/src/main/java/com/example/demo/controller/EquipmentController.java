package com.example.demo.controller;

import com.example.demo.model.Equipment;
import com.example.demo.model.EquipmentCreateDTO;
import com.example.demo.model.EquipmentUpdateDTO;
import com.example.demo.repository.EquipmentRepository;
import jakarta.validation.Valid;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.ResponseEntity;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.web.bind.annotation.*;

import java.util.List;
import java.util.UUID;

@CrossOrigin(origins = "*", maxAge = 3600)
@RestController
@RequestMapping("/api/equipment")
public class EquipmentController {

    @Autowired
    private EquipmentRepository equipmentRepository;

    @GetMapping
    public List<Equipment> getAllEquipment() {
        return equipmentRepository.findAll();
    }

    @GetMapping("/{id}")
    public ResponseEntity<Equipment> getEquipmentById(@PathVariable UUID id) {
        return equipmentRepository.findById(id)
                .map(ResponseEntity::ok)
                .orElse(ResponseEntity.notFound().build());
    }

    @PostMapping
    @PreAuthorize("hasRole('ADMIN')")
    public Equipment addEquipment(@Valid @RequestBody EquipmentCreateDTO dto) {

        Equipment newEquipment = new Equipment();

        newEquipment.setEquipmentName(dto.getEquipmentName());
        newEquipment.setCategory(dto.getCategory());
        newEquipment.setEquipmentCondition(dto.getEquipmentCondition());
        newEquipment.setTotalQuantity(dto.getTotalQuantity());

        newEquipment.setAvailableQuantity(dto.getTotalQuantity());
        newEquipment.setAvailable(dto.getTotalQuantity() > 0);
        return equipmentRepository.save(newEquipment);
    }

    @PutMapping("/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<Equipment> updateEquipment(@PathVariable UUID id,
                                                     @Valid @RequestBody EquipmentUpdateDTO dto) {

        return equipmentRepository.findById(id)
                .map(existingEquipment -> {

                    existingEquipment.setEquipmentName(dto.getEquipmentName());
                    existingEquipment.setCategory(dto.getCategory());
                    existingEquipment.setEquipmentCondition(dto.getEquipmentCondition());
                    existingEquipment.setTotalQuantity(dto.getTotalQuantity());
                    existingEquipment.setAvailableQuantity(dto.getAvailableQuantity());

                    existingEquipment.setAvailable(dto.getAvailableQuantity() > 0);

                    Equipment updatedEquipment = equipmentRepository.save(existingEquipment);
                    return ResponseEntity.ok(updatedEquipment);
                })
                .orElse(ResponseEntity.notFound().build());
    }

    @DeleteMapping("/{id}")
    @PreAuthorize("hasRole('ADMIN')")
    public ResponseEntity<?> deleteEquipment(@PathVariable UUID id) {
        return equipmentRepository.findById(id)
                .map(equipment -> {
                    equipmentRepository.delete(equipment);
                    return ResponseEntity.ok().build();
                })
                .orElse(ResponseEntity.notFound().build());
    }
}