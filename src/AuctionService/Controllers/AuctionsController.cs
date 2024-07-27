using AuctionService.Data;
using AuctionService.DTOs;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _context;
        private readonly IMapper _mapper;
        public AuctionsController(AuctionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAll()
        {
            var auctions = await _context.Auctions
                .AsNoTracking()
                .Include(x => x.Item)
                .OrderBy(x => x.Item.Make)
                .ToListAsync();

            return _mapper.Map<List<AuctionDto>>(auctions);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<AuctionDto>> GetById(Guid id)
        {
            var auction = await _context.Auctions
                .AsNoTracking()
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
                return NotFound();

            return _mapper.Map<AuctionDto>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>> Create(CreateAuctionDto auctionDto)
        {
            var auction = _mapper.Map<Auction>(auctionDto);
            //TODO: add current user as seller
            auction.Seller = "test";

            _context.Auctions.Add(auction);
            var result = await _context.SaveChangesAsync() > 0;

            if (!result) 
                return BadRequest("Could not save changes to the db");

            return CreatedAtAction(nameof(GetById), new { auction.Id }, _mapper.Map<AuctionDto>(auction));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<ActionResult<AuctionDto>> Update(Guid id, UpdateAuctionDto updateAuctionDto)
        {
            var auction = await _context.Auctions
                .Include(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
                return NotFound();

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage != 0 ? updateAuctionDto.Mileage : auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year != 0 ? updateAuctionDto.Year : auction.Item.Year;

            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest();

            return Ok();

        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var auction = await _context.Auctions
                .FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null)
                return NotFound();

            _context.Auctions.Remove(auction);
            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
                return BadRequest();

            return Ok();
        }
    }
}
