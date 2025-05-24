using UploadService.Data;
using UploadService.Models;
using Microsoft.EntityFrameworkCore;
using UploadService.DTOs;  

namespace UploadService.Repositories;

public class AudioRecordRepository
{
    private readonly AppDbContext _context;

    public AudioRecordRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AudioRecord> AddAudioRecordAsync(UploadRequest request)
    {
        using var memoryStream = new MemoryStream();
        await request.File.CopyToAsync(memoryStream);

        var audioRecord = new AudioRecord
        {
            SongName = request.SongName,
            Author = request.Author ?? "Неизвестный",
            FileData = memoryStream.ToArray(),
            FileName = request.File.FileName,
            ContentType = request.File.ContentType
        };

        _context.AudioRecords.Add(audioRecord);
        await _context.SaveChangesAsync();

        return audioRecord;
    }

    public async Task<List<AudioRecord>> GetAllAudioRecordsAsync()
    {
        return await _context.AudioRecords.ToListAsync();
    }

    public async Task<AudioRecord?> GetAudioRecordByIdAsync(Guid id)
    {
        return await _context.AudioRecords.FindAsync(id);
    }
}